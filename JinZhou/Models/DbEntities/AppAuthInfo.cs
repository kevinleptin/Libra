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
        public string AppId { get; set; }

        [MaxLength(160)]
        public string Code { get; set; }

        public DateTime ExpiredTime { get; set; }

        [MaxLength(80)]
        [Key]
        public string AuthorizerAppId { get; set; }

        /// <summary>
        /// 标志此用户授权给组件网站的状态，0为取消授权，1为已授权
        /// </summary>
        /// <value><c>true</c> if authorized; otherwise, <c>false</c>.</value>
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

        public AuthorizerInfo Authorizer
        {
            get;
            set;
        }

        [MaxLength(200)]
        public string AuthorizerUserName
        {
            get;
            set;
        }
    }
}
