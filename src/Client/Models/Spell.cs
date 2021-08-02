using System;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Spell
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(200)]
        public string DamageTraits { get; set; }

        public SpellType SpellType { get; set; }

        public SpellShape SpellShape { get; set; }

        public bool IsFriendlyFire { get; set; }

        [Range(0, 100)]
        public int Range { get; set; }

        [Range(1, 50)]
        public int Width { get; set; }

        [Range(1, 50)]
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

        public static Spell Empty =>
            new Spell
            {
                Range = 1,
                Width = 1,
                Height = 1,
            };
    }
}
