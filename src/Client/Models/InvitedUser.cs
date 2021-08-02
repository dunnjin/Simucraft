using System;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class InvitedUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string EmailNormalized { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public static InvitedUser Empty =>
            new InvitedUser
            {

            };
    }
}
