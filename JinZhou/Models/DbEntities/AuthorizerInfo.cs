﻿using System;
using System.ComponentModel.DataAnnotations;
namespace JinZhou.Models.DbEntities
{
    public class AuthorizerInfo
    {
        public AuthorizerInfo()
        {
        }

        [MaxLength(200)]
        [Key]
        public string UserName
        {
            get;
            set;
        }

        [MaxLength(80)]
        public string NickName
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string HeadImg
        {
            get;
            set;
        }


        public int ServiceType
        {
            get;
            set;
        }

        public int VerifyType
        {
            get;
            set;
        }



        [MaxLength(255)]
        public string PrincipalName
        {
            get;
            set;
        }

        public int BizStore
        {
            get;
            set;
        }

        public int BizScan
        {
            get;
            set;
        }

        public int BizPay
        {
            get;
            set;
        }

        public int BizCard
        {
            get;
            set;
        }

        public int BizShake
        {
            get;
            set;
        }

        [MaxLength(200)]
        public string Alias
        {
            get;
            set;
        }

        [MaxLength(255)]
        public string QrcodeUrl
        {
            get;
            set;
        }

    }
}
