using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simucraft.Client.Models
{
    public class GameCharacter
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CharacterId { get; set; }

        public string MaxHealthPoints { get; set; }

        public int HealthPoints { get; set; }

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

        public int Level { get; set; }

        public ICollection<Stat> Stats { get; set; } = new List<Stat>();

        public ICollection<GameCharacterSkill> Skills { get; set; } = new List<GameCharacterSkill>();

        public ICollection<Weapon> Weapons { get; set; } = new List<Weapon>();

        public decimal CarryingCapacity => (int)Math.Floor(this.Weapons.Sum(w => w.Weight));

        public static GameCharacter Empty =>
            new GameCharacter
            {

            };
    }
}
