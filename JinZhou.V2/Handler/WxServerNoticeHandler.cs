using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using JinZhou.V2.Models;
using JinZhou.V2.Services;
using Senparc.Weixin.Open;
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

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            var componentToken = ComponentTokenService.GetInstance().Token;
            componentToken.ComponentVerifyTicketCreateOn = DateTime.Now;
            componentToken.ComponentVerifyTicket = requestMessage.ComponentVerifyTicket;
            ComponentTokenService.GetInstance().Save();

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