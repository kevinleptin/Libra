﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JinZhou.V2.Models
{
    public class WxUserInfo
    {
        public WxUserInfo()
        {
        }

        [Key]
        [MaxLength(180)]
        public string OpenId
        {
            get;
            set;
        }

        [MaxLength(180)]
        public string NickName
        {
            get;
            set;
        }

        public int Sex
        {
            get;
            set;
        }

        [MaxLength(50)]
        public string Country
        {
            get;
            set;
        }

        [MaxLength(50)]
        public string Province
        {
            get;
            set;
        }

        [MaxLength(50)]
        public string City
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string HeadImgUrl
        {
            get;
            set;
        }

        [MaxLength(180)]
        public string UnionId
        {
            get;
            set;
        }

        [MaxLength(180)]
        public string AppId
        {
            get;
            set;
        }
    }
}