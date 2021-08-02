using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameCharacterRequest
    {
        public Guid Id { get; set; }

        public Guid CharacterId { get; set; }

        public Guid UserId { get; set; }

        public string MaxHealthPoints { get; set; }

        public long HealthPoints { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Movement { get; set; }

        /// <summary>
        /// Gets or sets whether the character should be removed when a new Map is being loaded.
        /// </summary>
        public bool IsFavorite { get; set; }

        public bool IsVisible { get; set; }

        public int TurnOrder { get; set; }

        public string MaxCarryingCapacity { get; set; }

        public decimal CarryingCapacity { get; set; }

        public int Level { get; set; }

        public ICollection<GameCharacterSkillRequest> Skills { get; set; } = new List<GameCharacterSkillRequest>();

        public ICollection<GameCharacterStatRequest> Stats { get; set; } = new List<GameCharacterStatRequest>();

        public ICollection<GameCharacterWeaponRequest> Weapons { get; set; } = new List<GameCharacterWeaponRequest>();
    }
}
