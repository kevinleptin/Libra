using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JinZhou.V2.Models.ViewModels
{
    public class HomeInstalledViewModel
    {
        public HomeInstalledViewModel()
        {
        }

        public string AuthorizerAppId
        {
            get;
            set;
        }

        public string AuthUrl
        {
            get;
            set;
        }
    }
}