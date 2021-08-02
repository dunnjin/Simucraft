using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class MapResponse
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public string RulesetName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool EnableFogOfWar { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<ColliisionTileResponse> CollisionTiles { get; set; } = new List<ColliisionTileResponse>();

        public ICollection<MapCharacterResponse> MapCharacters { get; set; } = new List<MapCharacterResponse>();
    }
}
