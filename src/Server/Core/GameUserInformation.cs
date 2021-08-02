using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameUserInformation
    {
        public int MaxPlayerCount { get; set; }

        public ICollection<AuthorizedUserResponse> AuthorizedUsers { get; set; } = new List<AuthorizedUserResponse>();

        public ICollection<InvitedUserResponse> InvitedUsers { get; set; } = new List<InvitedUserResponse>();
    }
}
