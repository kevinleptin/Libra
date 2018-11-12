using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using JinZhou.V2.Models;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou.V2.Services
{
    public class ComponentTokenService
    {
        public ComponentToken GetToken()
        {
            using(ApplicationDbContext _context = new ApplicationDbContext())
            {
                return _context.ComponentTokens.OrderByDescending(c => c.Id).First();
            }
        }

        private void Log(string msg)
        {
            string logFileName = DateTime.Now.ToFileTimeUtc().ToString() + "th" + Thread.CurrentThread.ManagedThreadId + ".txt";
            string absFileName = HostingEnvironment.MapPath("~/logs/" + logFileName);
            System.IO.File.WriteAllText(absFileName, msg);
        }

        public ComponentToken ForceRefresh()
        {
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                var componentToken = _context.ComponentTokens.OrderByDescending(c => c.Id).First();
                try
                {
                    var updatedToken = ComponentApi.GetComponentAccessToken(ConfigurationManager.AppSettings["AppId"],
                        ConfigurationManager.AppSettings["AppSecret"],
                        componentToken.ComponentVerifyTicket);
                    componentToken.ComponentAccessTokenCreateOn = DateTime.Now;
                    componentToken.ComponentAccessTokenExpiresIn = updatedToken.expires_in;
                    componentToken.ComponentAccessToken = updatedToken.component_access_token;
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }
                return componentToken;
            }
        }

        public void SaveVerifyToken(ComponentToken token)
        {
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                var tokenInDb = _context.ComponentTokens.OrderByDescending(c => c.Id).First();
                tokenInDb.ComponentVerifyTicket = token.ComponentVerifyTicket;
                tokenInDb.ComponentVerifyTicketCreateOn = token.ComponentVerifyTicketCreateOn;
                _context.SaveChanges();
            }
        }

        public void SaveAccessToken(ComponentToken token)
        {
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                var tokenInDb = _context.ComponentTokens.OrderByDescending(c => c.Id).First();
                tokenInDb.ComponentAccessToken = token.ComponentAccessToken;
                tokenInDb.ComponentAccessTokenCreateOn = token.ComponentAccessTokenCreateOn;
                tokenInDb.ComponentAccessTokenExpiresIn = token.ComponentAccessTokenExpiresIn;
                _context.SaveChanges();
            }
        }

        public void SavePreAuthCode(ComponentToken token)
        {
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                var tokenInDb = _context.ComponentTokens.OrderByDescending(c => c.Id).First();
                tokenInDb.PreAuthCode = token.PreAuthCode;
                tokenInDb.PreAuthCodeCreateOn = token.PreAuthCodeCreateOn;
                tokenInDb.PreAuthCodeExpiresIn = token.PreAuthCodeExpiresIn;
                _context.SaveChanges();
            }
        }
        
    }
}