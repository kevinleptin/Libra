using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.DbEntities
{
    public class BasicToken
    {
        public BasicToken()
        {
            TicketRefreshOn = DateTime.Now;
            AccessTokenRefreshOn = DateTime.Now;
            PreAuthCodeRefreshOn = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Ticket { get; set; }

        public DateTime TicketRefreshOn { get; set; }

        [MaxLength(160)]
        public string AccessToken { get; set; }

        public DateTime AccessTokenRefreshOn { get; set; }

        public int AccessTokenExpiresIn { get; set; }

        [MaxLength(80)]
        public string PreAuthCode { get; set; }

        public DateTime PreAuthCodeRefreshOn { get; set; }

        public int PreAuthCodeExpiresIn { get; set; }
    }
}
