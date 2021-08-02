using Simucraft.Server.Core;
using System;

namespace Simucraft.Server.Strategies
{
    public class RequestRollGameCharacter : IGameStateStrategyRequest
    {
        public Guid GameCharacterId { get; set; }

        public Guid RulesetEntityId { get; set; }

        public RulesetEntityType RulesetEntityType { get; set; }
    }
}
