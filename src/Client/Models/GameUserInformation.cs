using System;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class GameUserInformation
    {
        public int MaxPlayerCount { get; set; }

        public ICollection<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();

        public ICollection<InvitedUser> InvitedUsers { get; set; } = new List<InvitedUser>();
    }
}
