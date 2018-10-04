using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.CommEntity
{
    public abstract class WxCode
    {
        public WxCode()
        {
            RefreshOn = DateTime.Now;
        }
        public DateTime RefreshOn { get; set; }
        public int ExpiresIn { get; set; }

        /// <summary>
        /// secs秒内过期
        /// </summary>
        /// <param name="secs"></param>
        /// <returns></returns>
        public bool ExpireAfterSecs(int secs)
        {
            if (ExpiresIn == 0)
            {
                return true;
            }
            DateTime expireAt = RefreshOn.AddSeconds(ExpiresIn);
            DateTime estimateTime = DateTime.Now.AddSeconds(secs);
            return (estimateTime >= expireAt);
        }
    }
}
