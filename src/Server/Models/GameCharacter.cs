using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    [Owned]
    public class GameCharacter
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CharacterId { get; set; }

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

        /// <summary>
        /// Gets or sets whether the character should be rendered.
        /// </summary>
        public bool IsVisible { get; set; }

        public int TurnOrder { get; set; }

        public decimal CarryingCapacity { get; set; }

        public string MaxCarryingCapacity { get; set; }

        public int Level { get; set; }

        public ICollection<GameCharacterStat> Stats { get; set; } = new List<GameCharacterStat>();

        public ICollection<GameCharacterSkill> Skills { get; set; } = new List<GameCharacterSkill>();

        public ICollection<GameCharacterWeapon> Weapons { get; set; } = new List<GameCharacterWeapon>();
    }
}
