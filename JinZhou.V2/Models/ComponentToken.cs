using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JinZhou.V2.Models
{
    public class ComponentToken
    {
        public ComponentToken()
        {
            ComponentVerifyTicketCreateOn = DateTime.Now;
            ComponentAccessTokenCreateOn = DateTime.Now;
            PreAuthCodeCreateOn = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime ComponentVerifyTicketCreateOn { get; set; }

        [MaxLength(100)]
        public string ComponentVerifyTicket { get; set; }

        [MaxLength(160)]
        public string ComponentAccessToken { get; set; }
        public int ComponentAccessTokenExpiresIn { get; set; }
        public DateTime ComponentAccessTokenCreateOn { get; set; }

        [MaxLength(100)]
        public string PreAuthCode { get; set; }
        public int PreAuthCodeExpiresIn { get; set; }
        public DateTime PreAuthCodeCreateOn { get; set; }

    }
}