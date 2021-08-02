using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Character
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public string HealthPoints { get; set; }

        public int Movement { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string CarryingCapacity { get; set; }

        public int Level { get; set; }

        public ICollection<Guid> WeaponIds { get; set; } = new List<Guid>();

        public ICollection<Guid> EquipmentIds { get; set; } = new List<Guid>();

        public ICollection<Guid> SpellIds { get; set; } = new List<Guid>();

        public ICollection<CharacterStat> Stats { get; set; } = new List<CharacterStat>();

        public ICollection<CharacterSkill> Skills { get; set; } = new List<CharacterSkill>();
    }
}
