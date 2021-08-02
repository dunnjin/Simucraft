using Simucraft.Client.Common;
using Simucraft.Client.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Character
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [NumberExpression]
        [StringLength(50)]
        public string HealthPoints { get; set; }

        [Range(0, 100)]
        [Integer]
        public int Movement { get; set; }

        [Range(0, 1000)]
        [Integer]
        public int Vision { get; set; }

        [Range(1, 50)]
        [Integer]
        public int Width { get; set; }

        [Range(1, 50)]
        [Integer]
        public int Height { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [NumberExpression]
        [StringLength(50)]
        public string CarryingCapacity { get; set; }

        [Integer]
        [Range(1, 1000)]
        public int Level { get; set; }

        public ICollection<Guid> WeaponIds { get; set; } = new List<Guid>();

        public ICollection<Guid> EquipmentIds { get; set; } = new List<Guid>();

        public ICollection<Guid> SpellIds { get; set; } = new List<Guid>();

        public IList<Stat> Stats { get; set; } = new List<Stat>();

        public IList<CharacterSkill> Skills { get; set; } = new List<CharacterSkill>();

        public static Character Empty =>
            new Character
            {
                Width = 1,
                Height = 1,
                Movement = 6,
                Level = 1,
                HealthPoints = "10",
            };
    }
}
