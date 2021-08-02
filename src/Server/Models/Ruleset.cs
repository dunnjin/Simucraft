using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Ruleset : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MovementOffset { get; set; }

        public string TurnOrderExpression { get; set; }

        public bool AutoApplyDamage { get; set; }
    }
}
