using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Map : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RulesetId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool EnableFogOfWar { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public IList<CollisionTile> CollisionTiles { get; set; } = new List<CollisionTile>();

        public ICollection<MapCharacter> MapCharacters { get; set; } = new List<MapCharacter>();
        //public string CollisionLayerJson { get; set; }

        //public string MapCharactersJson { get; set; }
    }
}
