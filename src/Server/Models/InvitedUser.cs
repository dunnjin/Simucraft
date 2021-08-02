using Microsoft.EntityFrameworkCore;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class InvitedUser
    {
        public Guid Id { get; set; }

        public string EmailNormalized { get; set; }

        public string Key { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
