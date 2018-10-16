using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using JinZhou.Models;
using JinZhou.Models.CommEntity;
using JinZhou.Models.Configuration;
using Microsoft.Extensions.Options;
using JinZhou.Models.ViewModels;
using JinZhou.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Open.ComponentAPIs;
using JinZhou.Models.DbEntities;

namespace JinZhou.Controllers
{
    public class HomeController : Controller
    {
        private readonly WxConfig _wxConfig;
        private JzDbContext db = null;
        public HomeController(IOptions<WxConfig> wxConfig, JzDbContext context)
        {
            _wxConfig = wxConfig.Value;
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UserAuth(string code, string state, string appid)
        {
            try
            {
                if (string.IsNullOrEmpty(appid))
                {
                    return Content("无效的请求");
                }

                string wxAuthRedirectUri = _wxConfig.UserAuthRedirectUri;
                string wxAuthUrlFmt =
                    "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}&component_appid={3}#wechat_redirect";
                //state is null indicates it's first time to get here.
                if (string.IsNullOrEmpty(state))
                {
                    //第一次进入，跳转到微信授权页
                    string wxAuthUrl = string.Format(wxAuthUrlFmt, appid, HttpUtility.UrlEncode(wxAuthRedirectUri),
                        "wxAuth1stStep", _wxConfig.AppId);
                    LogService.GetInstance().AddLog("state", null, wxAuthUrl, "", "VISIT");
                    return Redirect(wxAuthUrl);
                }

                if (string.IsNullOrEmpty(code))
                {
                    // user reject the auth
                    return Content("用户未授权，无法继续。");
                }

                LogService.GetInstance().AddLog("/Home/UserAuth", null, "获得用户授权提供的code。开始获取accesstoken", "", "Info");
                //通过code换取access_token
                string wxAccessTokenUrlFmt =
                    "https://api.weixin.qq.com/sns/oauth2/component/access_token?appid={0}&code={1}&grant_type=authorization_code&component_appid={2}&component_access_token={3}";
                string wxAccessTokenUrl = string.Format(wxAccessTokenUrlFmt, appid, code, _wxConfig.AppId,
                    ComponentKeys.GetInstance().AccessData.AccessCode);
                LogService.GetInstance().AddLog("state", null, wxAccessTokenUrl, "", "VISIT");
                LogService.GetInstance().AddLog("state", null, "access token is "+ComponentKeys.GetInstance().AccessData.AccessCode, "", "AccessCode");
                string accessTokenJsonStr = string.Empty;

                var handler = new HttpClientHandler();

                handler.ServerCertificateCustomValidationCallback = new Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>(CheckValidationResult);
                HttpClient client = new HttpClient(handler);
                try
                {//TODO:解决SSL GET的问题
                    

                    accessTokenJsonStr =
                        client.GetStringAsync(wxAccessTokenUrl)
                            .Result; //Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(wxAccessTokenUrl, null);
                }
                catch (Exception reqEx)
                {
                    LogService.GetInstance().AddLog("state", null, "access token get failed "+reqEx.Message, null, "Exception");
                }

                var accessTokenJsonObj = JObject.Parse(accessTokenJsonStr);
                var accessCode = accessTokenJsonObj.GetValue("access_token");
                var openid = accessTokenJsonObj.GetValue("openid");

                LogService.GetInstance().AddLog("/Home/UserAuth", null, "获取到Access code。开始获取用户信息", "", "Info");
                //获取用户的基本信息
                string wxUserInfoUrlFmt =
                    "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
                string wxUserInfoUrl = string.Format(wxUserInfoUrlFmt, accessCode, openid);
                LogService.GetInstance().AddLog("state", null, wxUserInfoUrl, "", "VISIT");
                string userInfoJsonStr = client.GetStringAsync(wxUserInfoUrl).Result; //Senparc.CO2NET.HttpUtility.RequestUtility.HttpGet(wxUserInfoUrl, null);
                var userInfoJsonObj = JObject.Parse(userInfoJsonStr);
                var wxUserinfoEntity = new WxUserInfo()
                {
                    OpenId = userInfoJsonObj.GetValue("openid").ToString(),
                    NickName = userInfoJsonObj.GetValue("nickname").ToString(),
                    Sex = int.Parse(userInfoJsonObj.GetValue("sex").ToString()),
                    Country = userInfoJsonObj.GetValue("country").ToString(),
                    Province = userInfoJsonObj.GetValue("province").ToString(),
                    City = userInfoJsonObj.GetValue("city").ToString(),
                    HeadImgUrl = userInfoJsonObj.GetValue("headimgurl").ToString()
                };
                JToken unionIdProperty = null;
                if (userInfoJsonObj.TryGetValue("unionid", out unionIdProperty))
                {
                    wxUserinfoEntity.UnionId = unionIdProperty.ToString();
                }

                db.WxUserInfos.Add(wxUserinfoEntity);
                db.SaveChanges();

                return Content("您好，" + userInfoJsonObj.GetValue("nickname"));
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public IActionResult Install()
        {
            //授权成功后返回的 /Home/Installed?auth_code=queryauthcode@@@tKlkuvs2i5XuP3wloLDuauVHnQ4kZdU6LPczHEAarkABxUURgl9hOy_YHb_Ndsn8uu6j6Uv1za9q1ecmHi4MvQ&expires_in=3600
            LogService.GetInstance().AddLog("Home:Install", null, "View The Page", "", "Info");
            HomeInstallViewModel vm = new HomeInstallViewModel();
            vm.WxAppId = _wxConfig.AppId;
            vm.RedirectUri = _wxConfig.RedirectUri;
            vm.PreAuthCode = ComponentKeys.GetInstance().PreAuthData.PreAuthCode;
            return View(vm);
        }

        public IActionResult Installed(string auth_code, int expires_in)
        {
            LogService.GetInstance().AddLog("Home:Installed", null, "Auth succeed", "", "Info");

            var queryAuth = Senparc.Weixin.Open.ComponentAPIs.ComponentApi.QueryAuth(ComponentKeys.GetInstance().AccessData.AccessCode,
                _wxConfig.AppId, auth_code);

            string authorizerAppid = queryAuth.authorization_info.authorizer_appid;
            var authorizer = db.AppAuths.FirstOrDefault(c => c.AuthorizerAppId == authorizerAppid);
            if (authorizer == null || authorizer.Code != auth_code)
            {
                if (authorizer == null)
                {
                    authorizer = new AppAuthInfo();
                    authorizer.AppId = _wxConfig.AppId;
                    authorizer.AuthorizerAppId = queryAuth.authorization_info.authorizer_appid;
                    authorizer.Authorized = true;
                    authorizer.CreateOn = DateTime.Now;

                    var authorizerInfoResult = ComponentApi.GetAuthorizerInfo(ComponentKeys.GetInstance().AccessData.AccessCode, _wxConfig.AppId, queryAuth.authorization_info.authorizer_appid);
                    var authorizerInfo = authorizerInfoResult.authorizer_info;
                    var authorizerInfoEntity =
                        db.AuthorizerInfos.FirstOrDefault(c => c.UserName == authorizerInfo.user_name);
                    if (authorizerInfoEntity == null)
                    {
                        authorizerInfoEntity = new JinZhou.Models.DbEntities.AuthorizerInfo()
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
                    }

                    authorizer.Authorizer = authorizerInfoEntity;
                    //TODO: 这里应该存储以下信息，并自动刷新
                    //todo: queryAuth.authorization_info.authorizer_access_token
                    //TODO: queryAuth.authorization_info.authorizer_refresh_token 
                    AuthorizerToken token =
                        db.AuthorizerTokens.FirstOrDefault(c => c.AuthorizerAppId == authorizerAppid);
                    if (token == null)
                    {
                        token = new AuthorizerToken();
                        db.AuthorizerTokens.Add(token);
                    }

                    token.RefreshOn = DateTime.Now;
                    token.AuthorizerAccessToken = queryAuth.authorization_info.authorizer_access_token;
                    token.AuthorizerRefreshToken = queryAuth.authorization_info.authorizer_refresh_token;
                    token.ExpiredIn = queryAuth.authorization_info.expires_in;
                    db.SaveChanges();

                    //todo: 网站加入性能监控的组件，方便了解网站的运行状态

                    db.AppAuths.Add(authorizer);
                }
                //need update
                authorizer.Code = auth_code;
                authorizer.ExpiredTime = DateTime.Now.AddSeconds(queryAuth.authorization_info.expires_in);
                authorizer.LastUpdateOn = DateTime.Now;
                db.SaveChanges();
            }

            HomeInstalledViewModels vm = new HomeInstalledViewModels();
            vm.AuthorizerAppId = authorizerAppid;
            vm.AuthUrl = string.Format(_wxConfig.UserAuthEntryPointUriFmt, authorizerAppid);
            return View(vm);
        }

        public IActionResult WxComm()
        {
            //for test db connection
            CommEntityUpdater updater = new CommEntityUpdater(_wxConfig, db);

            HomeWxCommViewModel vm = new HomeWxCommViewModel();
            vm.AccessData = ComponentKeys.GetInstance().AccessData;
            vm.PreAuthData = ComponentKeys.GetInstance().PreAuthData;
            vm.VerifyData = ComponentKeys.GetInstance().VerifyData;
            vm.Logs = LogService.GetInstance().GetLogs();
            return View(vm);
        }

        public IActionResult Refresh()
        {
            int timeSpanToUpdate = 300; //提前5分钟进行update
            var almostExpireList = db.AuthorizerTokens.Where(c => c.RefreshOn.AddSeconds(c.ExpiredIn - timeSpanToUpdate) > DateTime.Now);
            foreach (var token in almostExpireList)
            {
               var refreshToken = ComponentApi.ApiAuthorizerToken(ComponentKeys.GetInstance().AccessData.AccessCode, _wxConfig.AppId,
                    token.AuthorizerAppId, token.AuthorizerRefreshToken);
               token.RefreshOn = DateTime.Now;
                token.AuthorizerAccessToken = refreshToken.authorizer_access_token;
                token.AuthorizerRefreshToken = refreshToken.authorizer_refresh_token;
                token.ExpiredIn = refreshToken.expires_in;
            }

            db.SaveChanges();

            return Content("Refresh Success");
        }
    }
}
