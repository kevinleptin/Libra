using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models.Configuration;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;

namespace JinZhou.Handler
{
    public class InsideMsgHandler : ThirdPartyMessageHandler
    {
        private readonly WxConfig _wxConfig;
        public InsideMsgHandler(Stream inputStream, WxConfig wxConfig, PostModel postmodel = null)
            : base(inputStream, postmodel)
        {
            _wxConfig = wxConfig;
        }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            CommEntityUpdater updater = new CommEntityUpdater(_wxConfig);
            updater.UpdateVerifyData(requestMessage.ComponentVerifyTicket);
            return base.OnComponentVerifyTicketRequest(requestMessage);
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
