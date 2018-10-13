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
using JinZhou.Models.DbEntities;
using JinZhou.Models.CommEntity;
using Senparc.Weixin.Open.ComponentAPIs;

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
            // create new entity
            AppAuthInfo appInfo = new AppAuthInfo();
            appInfo.AuthorizerAppId = requestMessage.AuthorizerAppid; // db table key
            appInfo.AppId = requestMessage.AppId; // 第三方平台的 appid
            appInfo.Authorized = true;
            appInfo.Code = requestMessage.AuthorizationCode;
            appInfo.ExpiredTime = requestMessage.AuthorizationCodeExpiredTime;
            appInfo.CreateOn = DateTime.Now;
            appInfo.LastUpdateOn = DateTime.Now;

            //,
            var authorizerInfoResult = ComponentApi.GetAuthorizerInfo(ComponentKeys.GetInstance().AccessData.AccessCode, _wxConfig.AppId, requestMessage.AuthorizerAppid);
            var authorizerInfo = authorizerInfoResult.authorizer_info;
            var authorizerInfoEntity = new JinZhou.Models.DbEntities.AuthorizerInfo()
            {
                UserName = authorizerInfo.user_name,
                NickName = authorizerInfo.nick_name,
                HeadImg = authorizerInfo.head_img,
                ServiceType = (int)authorizerInfo.service_type_info.id,
                VerifyType = (int)authorizerInfo.verify_type_info.id,
                PrincipalName = authorizerInfo.principal_name,
                BizStore = authorizerInfo.business_info.open_store,
                BizPay = authorizerInfo.business_info.open_pay,
                BizCard = authorizerInfo.business_info.open_card,
                BizScan = authorizerInfo.business_info.open_scan,
                BizShake = authorizerInfo.business_info.open_shake,
                Alias = authorizerInfo.alias,
                QrcodeUrl = authorizerInfo.qrcode_url
            };

            appInfo.Authorizer = authorizerInfoEntity;

            db.AppAuths.Add(appInfo);
            db.SaveChanges();
            return base.OnAuthorizedRequest(requestMessage);
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            string autherAppId = requestMessage.AuthorizerAppid;
            var appAuthInfo = db.AppAuths.FirstOrDefault(c => c.AuthorizerAppId == autherAppId);
            if(appAuthInfo != null)
            {
                appAuthInfo.LastUpdateOn = DateTime.Now;
                appAuthInfo.Authorized = false;
                db.SaveChanges();
            }
            return base.OnUnauthorizedRequest(requestMessage);
        }

        public override string OnUpdateAuthorizedRequest(RequestMessageUpdateAuthorized requestMessage)
        {
            string autherAppId = requestMessage.AuthorizerAppid;
            var appAuthInfo = db.AppAuths.FirstOrDefault(c => c.AuthorizerAppId == autherAppId);
            if (appAuthInfo != null)
            {
                appAuthInfo.LastUpdateOn = DateTime.Now;
                appAuthInfo.Authorized = true;
                appAuthInfo.AuthorizerAppId = requestMessage.AuthorizerAppid;
                appAuthInfo.Code = requestMessage.AuthorizationCode;
                appAuthInfo.ExpiredTime = requestMessage.AuthorizationCodeExpiredTime;
                db.SaveChanges();

                //todo: 增加authorizer的信息更新
            }


            return base.OnUpdateAuthorizedRequest(requestMessage);
        }
    }
}
