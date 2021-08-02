using System;

namespace Simucraft.Server.Models
{
    public class InvitedGame : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid GameId { get; set; }
    }
}
