using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using JinZhou.V2.Models;
using Senparc.Weixin.Open.ComponentAPIs;

namespace JinZhou.V2.Services
{
    public class ComponentTokenService
    {
        public ComponentToken Token { get; set; }
        
        private static object locker = new object();
        private static ComponentTokenService _componentTokenService = null;
        public DateTime LastSync { get; set; }
        private ComponentTokenService()
        {
            ApplicationDbContext _context = ApplicationDbContext.Create();
            Token = _context.ComponentTokens.OrderByDescending(c => c.Id).FirstOrDefault();
            LastSync = DateTime.Now;
            if (Token == null)
            {
                Token = new ComponentToken();
                _context.ComponentTokens.Add(Token);
                _context.SaveChanges();
            }
        }

        public static ComponentTokenService GetInstance()
        {
            if (_componentTokenService == null)
            {
                lock (locker)
                {
                    if (_componentTokenService == null)
                    {
                        _componentTokenService = new ComponentTokenService();
                    }
                }
            }

            return _componentTokenService;
        }

        internal ComponentToken ForceUpdate()
        {
            ApplicationDbContext _context = new ApplicationDbContext();
            Token = _context.ComponentTokens.OrderByDescending(c => c.Id).FirstOrDefault();
            LastSync = DateTime.Now;
            return Token;
        }

        public ComponentToken ForceRefresh()
        {
            var componentToken = ComponentTokenService.GetInstance().Token;
            //main server upadate 
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
            return componentToken;
        }

        /// <summary>
        /// Save updates on Token
        /// </summary>
        public void Save()
        {
            ApplicationDbContext _context = new ApplicationDbContext();
            var _token = _context.ComponentTokens.OrderByDescending(c => c.Id).FirstOrDefault();
            
            _token.ComponentAccessToken = Token.ComponentAccessToken;
            _token.ComponentAccessTokenCreateOn = Token.ComponentAccessTokenCreateOn;
            _token.ComponentAccessTokenExpiresIn = Token.ComponentAccessTokenExpiresIn;

            _token.ComponentVerifyTicket = Token.ComponentVerifyTicket;
            _token.ComponentVerifyTicketCreateOn = Token.ComponentVerifyTicketCreateOn;

            _token.PreAuthCode = Token.PreAuthCode;
            _token.PreAuthCodeCreateOn = Token.PreAuthCodeCreateOn;
            _token.PreAuthCodeExpiresIn = Token.PreAuthCodeExpiresIn;
            _context.SaveChanges();
        }
    }
}