using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class MapRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool EnableFogOfWar { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<CollisionTileRequest> CollisionTiles { get; set; } = new List<CollisionTileRequest>();

        public ICollection<MapCharacterRequest> MapCharacters { get; set; } = new List<MapCharacterRequest>();
    }
}
