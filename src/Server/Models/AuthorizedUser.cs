using Microsoft.EntityFrameworkCore;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class AuthorizedUser
    {
        public Guid UserId { get; set; }

        public string DisplayName { get; set; }

        public string EmailNormalized { get; set; }

        public bool IsConnected { get; set; }

        public string Role { get; set; }
    }
}
