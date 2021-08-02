using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class Effect
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public EffectType EffectType { get; set; }

        [Range(0, int.MaxValue)]
        public int Range { get; set; }

        [Range(0, int.MaxValue)]
        public int Width { get; set; }

        [Range(0, int.MaxValue)]
        public int Height { get; set; }

        public Formula HitChance { get; set; }

        public Formula ResistChance { get; set; }

        public Formula CriticalChance { get; set; }

        public Formula EffectFormula { get; set; }

        [StringLength(50)]
        public string Damage { get; set; }

        [StringLength(50)]
        public string CriticalDamage { get; set; }

        [StringLength(200)]
        public string DamageType { get; set; }

        public DamageShape DamageShape { get; set; }

        public DamageTarget DamageTarget { get; set; }

        public static Effect Empty
        {
            get
            {
                const string defaultOperator = ">";

                return new Effect
                {
                    Range = 1,
                    Width = 1,
                    Height = 1,
                    HitChance = new Formula
                    {
                        Operator = defaultOperator,
                    },
                    ResistChance = new Formula
                    {
                        Operator = defaultOperator,
                    },
                    CriticalChance = new Formula
                    {
                        Operator = "=",
                    },
                    EffectFormula = new Formula
                    {
                        Operator = "+="
                    },
                };
            }
        }
    }
}
