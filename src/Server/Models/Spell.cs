using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Models
{
    public class Spell : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public SpellType SkillType { get; set; }

        public SpellShape SkillShape { get; set; }

        public int Range { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Damage { get; set; }

        public string DamageOperator { get; set; }

        public string CriticalDamage { get; set; }

        public string HitChanceSelf { get; set; }

        public string HitChanceTarget { get; set; }

        public string HitChanceOperator { get; set; }

        public string CriticalChanceSelf { get; set; }

        public string CriticalChanceTarget { get; set; }

        public string CriticalChanceOperator { get; set; }

        public string ResistChanceSelf { get; set; }

        public string ResistChanceTarget { get; set; }

        public string ResistChanceOperator { get; set; }

        public string SkillTypes { get; set; }
    }
}
