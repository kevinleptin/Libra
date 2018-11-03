using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JinZhou.V2.Handler;
using JinZhou.V2.Models;
using JinZhou.V2.Services;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou.V2.Controllers.api
{
    public class TokenController : ApiController
    {
        private ApplicationDbContext _context;

        public TokenController()
        {
            _context = ApplicationDbContext.Create();
        }

        [HttpPost, Route("api/token/test")]
        public IHttpActionResult TestApi()
        {
            int step = 0;
            if (ComponentTokenService.GetInstance() == null)
            {
                step = 9001;
            }
            else
            {
                var componentToken = ComponentTokenService.GetInstance().Token;
                if (componentToken == null)
                {
                    step = 9002;
                }
                else if(componentToken.ComponentAccessToken == null)
                {
                    step = 9003;
                }
            }



            return Ok(step);
        }

        [HttpPost, Route("api/install/entryurl")]
        public IHttpActionResult RetrieveInstallUrl()
        {
            
            var WxAppId = ConfigurationManager.AppSettings["AppId"];
            var RedirectUri = ConfigurationManager.AppSettings["RedirectUri"];
            var PreAuthCode = ComponentTokenService.GetInstance().Token.PreAuthCode;

            string urlFormat = "https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid={0}&pre_auth_code={1}&redirect_uri={2}";
            string installUrl = string.Format(urlFormat, WxAppId, PreAuthCode, RedirectUri);

            var result = new {installUrl = installUrl};

            return Ok(result);
        }

        [HttpPost, Route("MPAccount/Auth")]
        public IHttpActionResult RefreshVerifyToken()
        {
            PostModel pm = new PostModel();
            pm.Timestamp = System.Web.HttpContext.Current.Request.QueryString["timestamp"];
            pm.Nonce = System.Web.HttpContext.Current.Request.QueryString["nonce"];
            pm.Msg_Signature = System.Web.HttpContext.Current.Request.QueryString["msg_signature"];

            pm.AppId = ConfigurationManager.AppSettings["AppId"];
            pm.Token = ConfigurationManager.AppSettings["Token"];
            pm.EncodingAESKey = ConfigurationManager.AppSettings["AesKey"];

            var Request = System.Web.HttpContext.Current.Request;
            WxServerNoticeHandler handler = new WxServerNoticeHandler(Request.InputStream, pm);
            handler.Execute();
            RefreshMpAccessCode();

            return Ok("success");
        }

        private void RefreshMpAccessCode()
        {
            var componentToken = ComponentTokenService.GetInstance().Token;
            string componentAppId = ConfigurationManager.AppSettings["AppId"];
            var mpTokenList = _context.MpTokens.ToList();
            foreach (var mpToken in mpTokenList)
            {
                if (DateTime.Now.AddSeconds(600) >= mpToken.RefreshOn.AddSeconds(mpToken.ExpiredIn))
                {
                  var refreshRlt =  ComponentApi.ApiAuthorizerToken(componentToken.ComponentAccessToken, componentAppId,
                        mpToken.MpAppId, mpToken.MpRefreshToken);
                    mpToken.ExpiredIn = refreshRlt.expires_in;
                    mpToken.MpAccessToken = refreshRlt.authorizer_access_token;
                    mpToken.MpRefreshToken = refreshRlt.authorizer_refresh_token;
                    mpToken.RefreshOn = DateTime.Now;
                    _context.SaveChanges();
                }
            }

        }
    }
}
