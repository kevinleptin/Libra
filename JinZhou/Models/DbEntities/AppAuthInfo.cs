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

        public bool Authorized { get; set; }
    }
}
