using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JinZhou.V2.Models;
using JinZhou.V2.Models.ViewModels;
using JinZhou.V2.Services;
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
                db.MpTokens.Add(token);
            }

            token.RefreshOn = DateTime.Now;
            token.MpAccessToken = queryAuth.authorization_info.authorizer_access_token;
            token.MpRefreshToken = queryAuth.authorization_info.authorizer_refresh_token;
            token.ExpiredIn = queryAuth.authorization_info.expires_in;
            token.BelongToMp = authorizerInfoEntity;
            db.SaveChanges();




            HomeInstalledViewModel vm = new HomeInstalledViewModel();
            vm.AuthorizerAppId = authorizerAppid;
            vm.AuthUrl = string.Format(ConfigurationManager.AppSettings["UserAuthEntryPointUriFmt"], authorizerAppid);
            return View(vm);
        }
    }
}