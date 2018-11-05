using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using JinZhou.V2.Migrations;
using JinZhou.V2.Models;
using JinZhou.V2.Services;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.ComponentAPIs;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;

namespace JinZhou.V2.Handler
{
    public class WxServerNoticeHandler : ThirdPartyMessageHandler
    {
        private ApplicationDbContext _Context;
        public WxServerNoticeHandler(Stream inputStream, PostModel postmodel = null)
            : base(inputStream, postmodel)
        {
           _Context = ApplicationDbContext.Create();
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
            var componentToken = ComponentTokenService.GetInstance().Token;
            componentToken.ComponentVerifyTicketCreateOn = DateTime.Now;
            componentToken.ComponentVerifyTicket = requestMessage.ComponentVerifyTicket;
            ComponentTokenService.GetInstance().Save();

            
            var lastSyncDate = ComponentTokenService.GetInstance().LastSync;
            if ((DateTime.Now - lastSyncDate).TotalHours >= 1)
            {
                ComponentTokenService.GetInstance().ForceUpdate();
            }

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
                }
                catch (Exception e)
                {
                    componentToken.ComponentAccessToken = e.Message;
                }

                ComponentTokenService.GetInstance().Save();
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
                }
                catch (Exception e2)
                {
                    componentToken.PreAuthCode = e2.Message;
                }
                ComponentTokenService.GetInstance().Save();
            }

            

            return base.OnComponentVerifyTicketRequest(requestMessage);
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