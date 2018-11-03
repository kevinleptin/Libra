﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using JinZhou.V2.Models;
using JinZhou.V2.Models.ViewModels;
using JinZhou.V2.Services;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou.V2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;

        public HomeController()
        {
            db = ApplicationDbContext.Create();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult UserAuth(string code, string state, string appid, string returnUrl)
        {
            int step = 0;
            try
            {
                if (string.IsNullOrEmpty(appid))
                {
                    return Content("无效的请求");
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Content("Error: can't find url parameter <b>returnUrl</b>");
                }

                if (returnUrl.Contains("%") == false)
                {
                    returnUrl = HttpUtility.UrlEncode(returnUrl).Replace("+", "%20");
                }

                //TODO: verify if returnUrl domain is legal or not.

                string componentAppId = ConfigurationManager.AppSettings["AppId"];
                if (ComponentTokenService.GetInstance() == null)
                {
                    step = 9001;
                }
                var componentToken = ComponentTokenService.GetInstance().Token;
                if (componentToken == null)
                {
                    step = 9002;
                }
                string wxAuthRedirectUri = ConfigurationManager.AppSettings["UserAuthRedirectUri"]+"?returnUrl="+returnUrl;
                string wxAuthUrlFmt =
                    "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}&component_appid={3}#wechat_redirect";
                //state is null indicates it's first time to get here.
                if (string.IsNullOrEmpty(state))
                {
                    //throw new Exception("wx based on "+ wxAuthRedirectUri);
                    //第一次进入，跳转到微信授权页
                    string wxAuthUrl = string.Format(wxAuthUrlFmt, appid, HttpUtility.UrlEncode(wxAuthRedirectUri).Replace("+", "%20"),
                        "wxAuth1stStep", componentAppId);

                    return Redirect(wxAuthUrl);
                }

                if (string.IsNullOrEmpty(code))
                {
                    // user reject the auth
                    return Content("用户未授权，无法继续。");
                }

                step = 1;
                //通过code换取access_token
                
                string wxAccessTokenUrlFmt =
                    "https://api.weixin.qq.com/sns/oauth2/component/access_token?appid={0}&code={1}&grant_type=authorization_code&component_appid={2}&component_access_token={3}";
                string wxAccessTokenUrl = string.Format(wxAccessTokenUrlFmt, appid, code, componentAppId,
                    componentToken.ComponentAccessToken);
                
                string accessTokenJsonStr = string.Empty;

                HttpClient client = new HttpClient();
                step = 2;
                    accessTokenJsonStr =
                        client.GetStringAsync(wxAccessTokenUrl)
                            .Result; //Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(wxAccessTokenUrl, null);

                step = 3;
                var accessTokenJsonObj = JObject.Parse(accessTokenJsonStr);
                var accessCode = accessTokenJsonObj.GetValue("access_token");
                var openid = accessTokenJsonObj.GetValue("openid");
                step = 4;
                //获取用户的基本信息
                string wxUserInfoUrlFmt =
                    "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
                string wxUserInfoUrl = string.Format(wxUserInfoUrlFmt, accessCode, openid);

                step = 5;
                string userInfoJsonStr = client.GetStringAsync(wxUserInfoUrl).Result; //Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(wxUserInfoUrl, null);
                var userInfoJsonObj = JObject.Parse(userInfoJsonStr);
                step = 6;
                string openIdStr = openid.ToString();
                step = 7;
                var wxUserinfoEntity = db.WxUserInfos.FirstOrDefault(c => c.OpenId == openIdStr);
                step = 8;
                if (wxUserinfoEntity == null)
                {
                    step = userInfoJsonObj == null ? 8000 : step;
                    step = userInfoJsonObj.GetValue("openid") == null ? 8001 : step;
                    step = userInfoJsonObj.GetValue("nickname") == null ? 8002 : step;
                    step = userInfoJsonObj.GetValue("sex") == null ? 8003 : step;
                    step = userInfoJsonObj.GetValue("country") == null ? 8004 : step;
                    step = userInfoJsonObj.GetValue("province") == null ? 8005 : step;
                    step = userInfoJsonObj.GetValue("city") == null ? 8006 : step;
                    step = userInfoJsonObj.GetValue("headimgurl") == null ? 8007 : step;
                    step = userInfoJsonObj.GetValue("uionid") == null ? 8008 : step;
                    
                    wxUserinfoEntity = new WxUserInfo()
                    {
                        OpenId = userInfoJsonObj.GetValue("openid").ToString(),
                        NickName = userInfoJsonObj.GetValue("nickname").ToString(),
                        Sex = int.Parse(userInfoJsonObj.GetValue("sex").ToString()),
                        Country = userInfoJsonObj.GetValue("country").ToString(),
                        Province = userInfoJsonObj.GetValue("province").ToString(),
                        City = userInfoJsonObj.GetValue("city").ToString(),
                        HeadImgUrl = userInfoJsonObj.GetValue("headimgurl").ToString()
                    };
                    step = 8009;
                    JToken unionIdProperty = null;
                    if (userInfoJsonObj.TryGetValue("unionid", out unionIdProperty))
                    {
                        step = 8010;
                        wxUserinfoEntity.UnionId = unionIdProperty.ToString();
                    }

                    step = 8011;
                    db.WxUserInfos.Add(wxUserinfoEntity);
                    db.SaveChanges();
                }

                step = 9;

                string decodeReturnUrl = HttpUtility.UrlDecode(returnUrl);
                //append infos
                step = 17;
                string redirectUrl = appendUserInfo(decodeReturnUrl, userInfoJsonObj);

                return Redirect(redirectUrl);
            }
            catch (Exception e)
            {
                return Content("step."+step+", "+ e.ToString());
            }
        }

        private string appendUserInfo(string url, JObject userInfo)
        {
            string nickName = userInfo.GetValue("nickname").ToString();
            if (!string.IsNullOrEmpty(nickName))
            {
                nickName = HttpUtility.UrlEncode(nickName).Replace("+", "%20");
            }

            string headImgUrl = userInfo.GetValue("headimgurl").ToString();
            if (!string.IsNullOrEmpty(headImgUrl))
            {
                headImgUrl = HttpUtility.UrlEncode(headImgUrl).Replace("+", "%20");
            }
            bool alreadyHasUrlParameter = url.Contains("?");
            url += alreadyHasUrlParameter
                ? "&nickname=" + nickName
                : "?nickname=" + nickName;
            url += "&openid=" + userInfo.GetValue("openid");
            url += "&headimgurl=" + headImgUrl;
            url += "&sex=" + userInfo.GetValue("sex");

            return url;
        }

        public ActionResult Install()
        {
            //授权成功后返回的 /Home/Installed?auth_code=queryauthcode@@@tKlkuvs2i5XuP3wloLDuauVHnQ4kZdU6LPczHEAarkABxUURgl9hOy_YHb_Ndsn8uu6j6Uv1za9q1ecmHi4MvQ&expires_in=3600
            
            HomeInstallViewModel vm = new HomeInstallViewModel();
            vm.WxAppId = ConfigurationManager.AppSettings["AppId"];
            vm.RedirectUri = ConfigurationManager.AppSettings["RedirectUri"];
            vm.PreAuthCode = ComponentTokenService.GetInstance().Token.PreAuthCode;
            return View(vm);
        }

        public ActionResult Installed(string auth_code, int expires_in)
        {
            var componentToken = ComponentTokenService.GetInstance().Token;
            string componentAppId = ConfigurationManager.AppSettings["AppId"];

            var queryAuth = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.QueryAuth(
                componentToken.ComponentAccessToken,
                componentAppId, auth_code);

            string authorizerAppid = queryAuth.authorization_info.authorizer_appid;


            var authorizerInfoResult = ComponentApi.GetAuthorizerInfo(componentToken.ComponentAccessToken,
                componentAppId, queryAuth.authorization_info.authorizer_appid);
            var authorizerInfo = authorizerInfoResult.authorizer_info;
            var authorizerInfoEntity = db.MpInfos.FirstOrDefault(c => c.UserName == authorizerInfo.user_name);
            if (authorizerInfoEntity == null)
            {
                authorizerInfoEntity = new MpInfo()
                {
                    UserName = authorizerInfo.user_name,
                    NickName = authorizerInfo.nick_name,
                    HeadImg = authorizerInfo.head_img,
                    ServiceType = (int) authorizerInfo.service_type_info.id,
                    VerifyType = (int) authorizerInfo.verify_type_info.id,
                    PrincipalName = authorizerInfo.principal_name,
                    BizStore = authorizerInfo.business_info.open_store,
                    BizPay = authorizerInfo.business_info.open_pay,
                    BizCard = authorizerInfo.business_info.open_card,
                    BizScan = authorizerInfo.business_info.open_scan,
                    BizShake = authorizerInfo.business_info.open_shake,
                    Alias = authorizerInfo.alias,
                    QrcodeUrl = authorizerInfo.qrcode_url
                };
                db.MpInfos.Add(authorizerInfoEntity);
            }

            MpToken token =
                db.MpTokens.FirstOrDefault(c => c.MpAppId == authorizerAppid);
            if (token == null)
            {
                token = new MpToken();
                token.MpAppId = authorizerAppid;
                db.MpTokens.Add(token);
            }
            
            token.RefreshOn = DateTime.Now;
            token.MpAccessToken = queryAuth.authorization_info.authorizer_access_token;
            token.MpRefreshToken = queryAuth.authorization_info.authorizer_refresh_token;
            token.ExpiredIn = queryAuth.authorization_info.expires_in;
            token.BelongToMp = authorizerInfoEntity;

            db.SaveChanges();




            //HomeInstalledViewModel vm = new HomeInstalledViewModel();
            //vm.AuthorizerAppId = authorizerAppid;
            //vm.AuthUrl = string.Format(ConfigurationManager.AppSettings["UserAuthEntryPointUriFmt"], authorizerAppid);
            string redirectUrl = string.Format(ConfigurationManager.AppSettings["InstallSuccessUrl"], authorizerAppid);
            return Redirect(redirectUrl);
        }
    }
}