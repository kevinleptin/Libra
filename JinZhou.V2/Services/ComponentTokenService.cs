using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JinZhou.V2.Models;

namespace JinZhou.V2.Services
{
    public class ComponentTokenService
    {
        public ComponentToken Token { get; set; }
        private ApplicationDbContext _context { get; set; }
        private static object locker = new object();
        private static ComponentTokenService _componentTokenService = null;
        public DateTime LastSync { get; set; }
        private ComponentTokenService()
        {
           _context = ApplicationDbContext.Create();
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
            Token = _context.ComponentTokens.OrderByDescending(c => c.Id).FirstOrDefault();
            LastSync = DateTime.Now;
            return Token;
        }

        /// <summary>
        /// Save updates on Token
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}