using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class RulesetResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSharing { get; set; }

        public int Likes { get; set; }

        public int MovementOffset { get; set; }

        public string CriticalModifier { get; set; }

        public string TurnOrderExpression { get; set; }

        public bool AutoApplyDamage { get; set; }
    }
}
