using Senparc.Weixin.Open.Entities.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JinZhou.V2.Handler;

namespace JinZhou.V2.Controllers.api
{
    public class TokenController : ApiController
    {
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
            
            return Ok("success");
        }
    }
}
