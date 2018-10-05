using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Models.CommEntity;

namespace JinZhou.Models.ViewModels
{
    public class HomeWxCommViewModel
    {
        public ComponentAccessData AccessData { get; set; }
        public ComponentVerifyData VerifyData { get; set; }
        public ComponentPreAuthData PreAuthData { get; set; }
        public List<Log> Logs { get; set; }
    }
}
