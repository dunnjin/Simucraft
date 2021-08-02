using System;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class RulesetInformation
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public bool IsOwner { get; set; }

        public string TurnOrderFormula { get; set; }

        public int MovementOffset { get; set; }

        public ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();

        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

        public ICollection<Spell> Spells { get; set; } = new List<Spell>();

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public ICollection<Character> Characters { get; set; } = new List<Character>();

        public ICollection<Map> Maps { get; set; } = new List<Map>();

        public static RulesetInformation Empty =>
            new RulesetInformation
            {

            };
    }
}
