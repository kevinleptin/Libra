using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models.CommEntity;
using JinZhou.Models.Configuration;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou
{
    /// <summary>
    /// 此类负责更新维护所有和微信服务器通信的code和key
    /// </summary>
    public class CommEntityUpdater
    {
        private readonly WxConfig _wxConfig;

        public CommEntityUpdater(WxConfig wxConfig)
        {
            _wxConfig = wxConfig;
        }
        public void UpdateVerifyData(string tikect)
        {
            var vd = ComponentKeys.GetInstance().VerifyData;
            vd.Ticket = tikect;
            vd.RefreshOn = DateTime.Now;
            vd.ExpiresIn = 600;
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
        }

        public void UpdatePreAuthCode()
        {
            var pac = ComponentKeys.GetInstance().PreAuthData;
            var pacRlt =
                ComponentApi.GetPreAuthCode(_wxConfig.AppId, ComponentKeys.GetInstance().AccessData.AccessCode);
            pac.PreAuthCode = pacRlt.pre_auth_code;
            pac.ExpiresIn = pacRlt.expires_in;
            pac.RefreshOn = DateTime.Now;
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
        }
    }
}
