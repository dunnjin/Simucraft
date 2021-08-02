using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class RulesetRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int MovementOffset { get; set; }

        public string TurnOrderExpression { get; set; }

        public bool AutoApplyDamage { get; set; }
    }
}
