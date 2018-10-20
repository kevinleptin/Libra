using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JinZhou.V2.Models
{
    /// <summary>
    /// Mp's Token
    /// </summary>
    public class MpToken
    {
        public MpToken()
        {
            CreateOn = DateTime.Now;
            RefreshOn = DateTime.Now;
        }
        [Key]
        [MaxLength(180)]
        public string MpAppId { get; set; }
        [MaxLength(180)]
        public string MpAccessToken { get; set; }
        [MaxLength(180)]
        public string MpRefreshToken { get; set; }
        public int ExpiredIn { get; set; }
        public DateTime RefreshOn { get; set; }
        public DateTime CreateOn { get; set; }

        public MpInfo BelongToMp { get; set; }

        [MaxLength(200)]
        public string BelongToMpUserName { get; set; }
    }
}