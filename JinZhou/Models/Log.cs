using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models
{
    public class Log
    {
        // 5W 1H
        public DateTime When { get; set; }
        /// <summary>
        /// 记录IP or ThreadID
        /// </summary>
        public string Who { get; set; }
        /// <summary>
        /// 所在函数名
        /// </summary>
        public string Where { get; set; }
        public string What { get; set; }
        /// <summary>
        /// 记录exception等
        /// </summary>
        public string Why { get; set; }
        /// <summary>
        /// 记录日志等级
        /// </summary>
        public string How { get; set; }
    }
}
