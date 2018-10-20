using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JinZhou.V2.Models.ViewModels
{
    public class HomeInstallViewModel
    {
        public string WxAppId { get; set; }
        public string PreAuthCode { get; set; }

        public string RedirectUri { get; set; }
    }
}