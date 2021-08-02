using System;

namespace Simucraft.Server.Models
{
    public class Weapon : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public int Range { get; set; }

        public string Damage { get; set; }

        public string DamageOperator { get; set; }

        public string CriticalDamage { get; set; }

        public string HitChanceSelf { get; set; }

        public string HitChanceTarget { get; set; }

        public string HitChanceOperator { get; set; }

        public string CriticalChanceSelf { get; set; }

        public string CriticalChanceTarget { get; set; }

        public string CriticalChanceOperator { get; set; }

        public string DamageTypes { get; set; }

        public decimal Weight { get; set; }

        public string Cost { get; set; }
    }
}
