using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models;
using JinZhou.Models.Configuration;
using JinZhou.Services;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;

namespace JinZhou.Handler
{
    public class InsideMsgHandler : ThirdPartyMessageHandler
    {
        private readonly WxConfig _wxConfig;
        private JzDbContext db = null;
        public InsideMsgHandler(JzDbContext db, Stream inputStream, WxConfig wxConfig, PostModel postmodel = null)
            : base(inputStream, postmodel)
        {
            this.db = db;
            _wxConfig = wxConfig;
        }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            LogService.GetInstance().AddLog("OnComponentVerifyTicketRequest", null, "Begin Update Verify Ticket", "", "Info");
            LogService.GetInstance().AddLog("OnComponentVerifyTicketRequest", null, "verify ticket is "+requestMessage.ComponentVerifyTicket, "", "Info");
            CommEntityUpdater updater = new CommEntityUpdater(_wxConfig, db);
            updater.UpdateVerifyData(requestMessage.ComponentVerifyTicket);
            LogService.GetInstance().AddLog("OnComponentVerifyTicketRequest", null, "End Update Verify Ticket", "", "Info");
            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        public override void OnExecuting()
        {
            try
            {
                base.OnExecuting();
            }
            catch (Exception ex)
            {
                LogService.GetInstance().AddLog("MessageHandler:OnExecuting", null, "OnExecuting", ex.Message, "Error");
            }
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
