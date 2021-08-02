using System;

namespace Simucraft.Client.Models
{
    public class RequestRollGameCharacter
    {
        public Guid GameCharacterId { get; set; }

        public Guid RulesetEntityId { get; set; }

        public RulesetEntityType RulesetEntityType { get; set; }
    }
}
