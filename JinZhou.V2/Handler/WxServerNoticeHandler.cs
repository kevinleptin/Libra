using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using JinZhou.V2.Migrations;
using JinZhou.V2.Models;
using JinZhou.V2.Services;
using Newtonsoft.Json;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;

namespace JinZhou.V2.Handler
{
    public class WxServerNoticeHandler : ThirdPartyMessageHandler
    {
        public WxServerNoticeHandler(Stream inputStream, PostModel postmodel = null)
            : base(inputStream, postmodel)
        {
          
        }

        private bool ExpiresIn(DateTime expireAt, int seconds)
        {
            if (seconds == 0)
            {
                return true;
            }
            DateTime estimateTime = DateTime.Now.AddSeconds(seconds);
            return (estimateTime >= expireAt);
        }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            ComponentTokenService cts = new ComponentTokenService();
            var componentToken = cts.GetToken();
            componentToken.ComponentVerifyTicketCreateOn = DateTime.Now;
            componentToken.ComponentVerifyTicket = requestMessage.ComponentVerifyTicket;
            cts.SaveVerifyToken(componentToken);
            

            var expiredTime =
                componentToken.ComponentAccessTokenCreateOn.AddSeconds(componentToken.ComponentAccessTokenExpiresIn);
            if (ExpiresIn(expiredTime, 1200))
            { //Refresh the token before 1200 seconds when it expired
                try
                {
                    var updatedToken = ComponentApi.GetComponentAccessToken(ConfigurationManager.AppSettings["AppId"],
                        ConfigurationManager.AppSettings["AppSecret"],
                        componentToken.ComponentVerifyTicket);
                    componentToken.ComponentAccessTokenCreateOn = DateTime.Now;
                    componentToken.ComponentAccessTokenExpiresIn = updatedToken.expires_in;
                    componentToken.ComponentAccessToken = updatedToken.component_access_token;
                    cts.SaveAccessToken(componentToken);
                    Log("update access token to " + JsonConvert.SerializeObject(componentToken));
                }
                catch (Exception e)
                {
                    Log(e.ToString(), true);
                }
            }

            expiredTime = componentToken.PreAuthCodeCreateOn.AddSeconds(componentToken.PreAuthCodeExpiresIn);
            if (ExpiresIn(expiredTime, 1200))
            {
                try
                {
                    var updatedCode = ComponentApi.GetPreAuthCode(ConfigurationManager.AppSettings["AppId"],
                        componentToken.ComponentAccessToken);
                    componentToken.PreAuthCodeExpiresIn = updatedCode.expires_in;
                    componentToken.PreAuthCode = updatedCode.pre_auth_code;
                    componentToken.PreAuthCodeCreateOn = DateTime.Now;
                    cts.SavePreAuthCode(componentToken);
                    Log("update preauth to " + JsonConvert.SerializeObject(componentToken));
                    
                }
                catch (Exception e2)
                {
                    Log(e2.ToString(), true);
                }
            }

            

            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        private void Log(string msg, bool isError = false)
        {
            string logFileName = (isError?"":"upt")+DateTime.Now.ToFileTimeUtc().ToString() + "th" + Thread.CurrentThread.ManagedThreadId + ".txt";
            string absFileName = HostingEnvironment.MapPath("~/logs/" + logFileName);
            System.IO.File.WriteAllText(absFileName, msg);
        }

        public override void OnExecuting()
        {
            base.OnExecuting();
        }

        public override string OnAuthorizedRequest(RequestMessageAuthorized requestMessage)
        {
            
            return base.OnAuthorizedRequest(requestMessage);
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            
            return base.OnUnauthorizedRequest(requestMessage);
        }

        public override string OnUpdateAuthorizedRequest(RequestMessageUpdateAuthorized requestMessage)
        {
            return base.OnUpdateAuthorizedRequest(requestMessage);
        }
    }
}