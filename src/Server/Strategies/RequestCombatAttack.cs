using Simucraft.Server.Core;
using System;

namespace Simucraft.Server.Strategies
{
    public class RequestCombatAttack : IGameStateStrategyRequest
    {
        public Guid GameCharacterId { get; set; }

        public Guid RulesetEntityId { get; set; }

        public RulesetEntityType RulesetEntityType { get; set; }

        public int TargetX { get; set; }

        public int TargetY { get; set; }
    }
}
