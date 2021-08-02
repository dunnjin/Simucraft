using Simucraft.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Map
    {
        public Guid Id { get; set; }

        [RequiredGuid(ErrorMessage = "A Ruleset is required.")]
        public Guid RulesetId { get; set; }

        public string RulesetName { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; } 

        [Required(ErrorMessage = "An Image is required.")]
        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public int VisionDistance { get; set; }

        public ICollection<CollisionTile> CollisionTiles { get; set; } = new List<CollisionTile>();

        public ICollection<MapCharacter> MapCharacters { get; set; } = new List<MapCharacter>();

        public static Map Empty =>
            new Map
            {
                TileHeight = 64,
                TileWidth = 64,
            };
    }
}
