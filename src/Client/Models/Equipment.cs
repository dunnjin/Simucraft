using Simucraft.Client.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Equipment
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

        public EquipmentType EquipmentTypeParsed
        {
            get => Enum.Parse<EquipmentType>(this.EquipmentType);
            set => this.EquipmentType = value.ToString();
        }

        public string EquipmentType { get; set; }

        [Range(1, 100)]
        public int Range { get; set; }

        [StringLength(50)]
        public string Cost { get; set; }

        [Range(0, 100000)]
        public decimal Weight { get; set; }

        [StringLength(200)]
        public string DamageTraits { get; set; }

        [StringLength(200)]
        public string ResistanceTraits { get; set; }

        [StringLength(200)]
        public string ImmunityTraits { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string DamageExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalDamageExpression { get; set; }

        [Required]
        [StringLength(5)]
        public string DamageOperatorExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string HitChanceSelfExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string HitChanceTargetExpression { get; set; }

        [Required]
        [StringLength(5)]
        public string HitChanceOperatorExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalHitChanceSelfExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string CriticalHitChanceTargetExpression { get; set; }

        [Required]
        [StringLength(5)]
        public string CriticalHitChanceOperatorExpression { get; set; }

        public ICollection<EquipmentExpression> PassiveExpressions { get; set; } = new List<EquipmentExpression>();

        public static Equipment Empty =>
            new Equipment
            {
                CriticalHitChanceOperatorExpression = ">",
                DamageOperatorExpression = "+",
                HitChanceOperatorExpression = ">",
                Range = 1,
                EquipmentTypeParsed = Models.EquipmentType.Active,
            };
    }
}
