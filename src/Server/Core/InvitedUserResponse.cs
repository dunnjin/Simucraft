using System;

namespace Simucraft.Server.Core
{
    public class InvitedUserResponse
    {
        public string EmailNormalized { get; set; }

        public string Key { get; set; }

        public DateTime ExpirationDateTime { get; set; }
    }
}
