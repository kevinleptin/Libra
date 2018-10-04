using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.CommEntity
{
    /// <summary>
    /// 10分钟刷新一次 Component Verify Ticket
    /// </summary>
    public class ComponentVerifyData : WxCode
    {
        public string Ticket { get; set; }
        
    }
}
