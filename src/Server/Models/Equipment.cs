using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Equipment : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public EquipmentType EquipmentType { get; set; }

        public int Range { get; set; }

        public string Cost { get; set; }

        public decimal Weight { get; set; }

        public string DamageTraits { get; set; }

        public string ResistanceTraits { get; set; }

        public string ImmunityTraits { get; set; }

        public string DamageExpression { get; set; }

        public string CriticalDamageExpression { get; set; }

        public string DamageOperatorExpression { get; set; }

        public string HitChanceSelfExpression { get; set; }

        public string HitChanceTargetExpression { get; set; }

        public string HitChanceOperatorExpression { get; set; }

        public string CriticalHitChanceSelfExpression { get; set; }

        public string CriticalHitChanceTargetExpression { get; set; }

        public string CriticalHitChanceOperatorExpression { get; set; }

        public ICollection<EquipmentExpression> PassiveExpressions { get; set; } = new List<EquipmentExpression>();
    }
}
