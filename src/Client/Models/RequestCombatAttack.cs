using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class RequestCombatAttack
    {
        public Guid GameCharacterId { get; set; }

        public Guid RulesetEntityId { get; set; }

        public RulesetEntityType RulesetEntityType { get; set; }

        public int TargetX { get; set; }

        public int TargetY { get; set; }
    }
}
