using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.DbEntities
{
    public class AppAuthInfo
    {
        [MaxLength(80)]
        [Key]
        public string AppId { get; set; }

        [MaxLength(160)]
        public string Code { get; set; }

        public DateTime ExpiredTime { get; set; }

        [MaxLength(80)]
        public string AuthorizerAppId { get; set; }

        public bool Authorized { get; set; }

        public DateTime CreateOn
        {
            get;
            set;
        }
        public DateTime LastUpdateOn
        {
            get;
            set;
        }
    }
}
