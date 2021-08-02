using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class CharacterResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string ImageUrl { get; set; }

        public string HealthPoints { get; set; }

        public int Movement { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string CarryingCapacity { get; set; }

        public int Level { get; set; }

        public ICollection<Guid> WeaponIds { get; set; } = new List<Guid>();

        public ICollection<Guid> EquipmentIds { get; set; } = new List<Guid>();

        public ICollection<Guid> SpellIds { get; set; } = new List<Guid>();

        public ICollection<CharacterStatResponse> Stats { get; set; } = new List<CharacterStatResponse>();

        public ICollection<CharacterSkillResponse> Skills { get; set; } = new List<CharacterSkillResponse>();
    }
}
