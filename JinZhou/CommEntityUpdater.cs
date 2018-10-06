using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models;
using JinZhou.Models.CommEntity;
using JinZhou.Models.Configuration;
using JinZhou.Models.DbEntities;
using JinZhou.Services;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou
{
    /// <summary>
    /// 此类负责更新维护所有和微信服务器通信的code和key
    /// </summary>
    public class CommEntityUpdater
    {
        private readonly WxConfig _wxConfig;
        private JzDbContext db = null;
        private BasicToken basicToken = null;

        public CommEntityUpdater(WxConfig wxConfig, JzDbContext db)
        {
            _wxConfig = wxConfig;
            this.db = db;
            basicToken = db.BasicTokens.FirstOrDefault();
            if (basicToken == null)
            {
                LogService.GetInstance().AddLog("CommEntityUpdater:ctor", null, "Create a new basic token record", "", "Info");
                basicToken = new BasicToken();
                db.BasicTokens.Add(basicToken);
                db.SaveChanges();
            }
            else if(string.IsNullOrEmpty(ComponentKeys.GetInstance().VerifyData.Ticket))
            {
                //load token from db to memory when component ticket is null
                ComponentKeys.GetInstance().VerifyData.Ticket = basicToken.Ticket;
                ComponentKeys.GetInstance().VerifyData.RefreshOn = basicToken.TicketRefreshOn;

                ComponentKeys.GetInstance().AccessData.AccessCode = basicToken.AccessToken;
                ComponentKeys.GetInstance().AccessData.ExpiresIn = basicToken.AccessTokenExpiresIn;
                ComponentKeys.GetInstance().AccessData.RefreshOn = basicToken.AccessTokenRefreshOn;

                ComponentKeys.GetInstance().PreAuthData.PreAuthCode = basicToken.PreAuthCode;
                ComponentKeys.GetInstance().PreAuthData.RefreshOn = basicToken.PreAuthCodeRefreshOn;
                ComponentKeys.GetInstance().PreAuthData.ExpiresIn = basicToken.PreAuthCodeExpiresIn;
            }
        }
        public void UpdateVerifyData(string tikect)
        {
            LogService.GetInstance().AddLog("UpdateVerifyData", null, "Begin update verify data","","Info");
            var vd = ComponentKeys.GetInstance().VerifyData;
            vd.Ticket = tikect;
            vd.RefreshOn = DateTime.Now;
            vd.ExpiresIn = 600;
            //store to db
            basicToken.Ticket = tikect;
            basicToken.TicketRefreshOn = vd.RefreshOn;

            Update();
        }

        public void UpdateAccessData()
        {
            var ad = ComponentKeys.GetInstance().AccessData;
            var atRlt = ComponentApi.GetComponentAccessToken(_wxConfig.AppId, _wxConfig.AppSecret,
                ComponentKeys.GetInstance().VerifyData.Ticket);
            ad.AccessCode = atRlt.component_access_token;
            ad.ExpiresIn = atRlt.expires_in;
            ad.RefreshOn = DateTime.Now;

            //store to db
            basicToken.AccessToken = ad.AccessCode;
            basicToken.AccessTokenRefreshOn = ad.RefreshOn;
            basicToken.AccessTokenExpiresIn = ad.ExpiresIn;
        }

        public void UpdatePreAuthCode()
        {
            var pac = ComponentKeys.GetInstance().PreAuthData;
            var pacRlt =
                ComponentApi.GetPreAuthCode(_wxConfig.AppId, ComponentKeys.GetInstance().AccessData.AccessCode);
            pac.PreAuthCode = pacRlt.pre_auth_code;
            pac.ExpiresIn = pacRlt.expires_in;
            pac.RefreshOn = DateTime.Now;

            //store to db
            basicToken.PreAuthCode = pac.PreAuthCode;
            basicToken.PreAuthCodeRefreshOn = pac.RefreshOn;
            basicToken.PreAuthCodeExpiresIn = pac.ExpiresIn;
        }

        public void Update()
        {
            if (ComponentKeys.GetInstance().AccessData.ExpireAfterSecs(600))
            {
                UpdateAccessData();
            }

            if (ComponentKeys.GetInstance().PreAuthData.ExpireAfterSecs(600))
            {
                UpdatePreAuthCode();
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                LogService.GetInstance().AddLog("CommEntityUpdater:Update", null, "Saving changes to db", ex.Message, "Error");
            }
        }
    }
}
