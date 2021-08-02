using System;

namespace Simucraft.Server.Core
{
    public class AuthorizedUserRequest
    {
        public Guid UserId { get; set; }

        public string DisplayName { get; set; }

        public string EmailNormalized { get; set; }
    }
}
