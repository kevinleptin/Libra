using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.Configuration
{
    public class WxConfig
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string RedirectUri { get; set; }

        public string Token { get; set; }
        public string AesKey { get; set; }

        public string UserAuthRedirectUri
        {
            get;
            set;
        }

        public string UserAuthEntryPointUriFmt
        {
            get;
            set;
        }
    }
}
