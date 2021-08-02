using System;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class GameInformation
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid? MapId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public string ImageUrl { get; set; }

        public bool IsLocked { get; set; }

        public IList<CollisionTile> CollisionTiles { get; set; } = new List<CollisionTile>();

        public static GameInformation Empty =>
            new GameInformation
            {

            };
    }
}
