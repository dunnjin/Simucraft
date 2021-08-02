using Simucraft.Client.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Simucraft.Client.Models
{
    /// <summary>
    /// Represents the container for all rules and interactions for gameplay.
    /// This stores all available interactable content that can be used in a campaign.
    /// </summary>
    public class Ruleset
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int MovementOffset { get; set; }

        [Required]
        [StringLength(200)]
        [NumberExpression]
        public string TurnOrderExpression { get; set; }

        public bool AutoApplyDamage { get; set; }

        public ICollection<Character> Characters { get; set; } = new List<Character>();

        public ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();

        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

        public ICollection<Spell> Spells { get; set; } = new List<Spell>();

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public static Ruleset Empty =>
            new Ruleset
            {

            };
    }
}
