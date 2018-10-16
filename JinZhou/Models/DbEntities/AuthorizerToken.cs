using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.DbEntities
{
    public class AuthorizerToken
    {
        public AuthorizerToken()
        {
            CreateOn = DateTime.Now;
            RefreshOn = DateTime.Now;
        }
        [Key]
        [MaxLength(180)]
        public string AuthorizerAppId { get; set; }
        [MaxLength(180)]
        public string AuthorizerAccessToken { get; set; }
        [MaxLength(180)]
        public string AuthorizerRefreshToken { get; set; }
        public int ExpiredIn { get; set; }
        public DateTime RefreshOn { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
