using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Pages;
using System;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Weapon
    {
        public Guid Id { get; set; }

        // TODO: Make a GameWeapon class
        public Guid WeaponId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Range(1, 1000)]
        [Integer]
        public int Range { get; set; }

        [Required]
        [StringLength(200)]
        [NumberExpression]
        public string Damage { get; set; }

        [StringLength(5)]
        public string DamageOperator { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalDamage { get; set; }

        [Required(ErrorMessage = "The Hit Chance field is required.")]
        [StringLength(200)]
        [NumberExpression]
        public string HitChanceSelf { get; set; }

        [Required(ErrorMessage = "The Hit Chance field is required.")]
        [StringLength(200)]
        [NumberExpression]
        public string HitChanceTarget { get; set; }

        [StringLength(5)]
        public string HitChanceOperator { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalChanceSelf { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalChanceTarget { get; set; }

        [StringLength(5)]
        public string CriticalChanceOperator { get; set; }

        [StringLength(200)]
        public string DamageTypes { get; set; }

        [Range(0, 1000)]
        public decimal Weight { get; set; }

        [StringLength(50)]
        public string Cost { get; set; }

        public static Weapon Empty =>
            new Weapon
            {
                Range = 1,
                DamageOperator = "*",
                HitChanceOperator = ">",
                CriticalChanceOperator = "="
            };
    }
}
