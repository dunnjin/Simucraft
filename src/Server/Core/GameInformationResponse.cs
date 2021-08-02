using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameInformationResponse
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

        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameStateMode { get; set; }

        public bool IsLocked { get; set; }

        public IList<ColliisionTileResponse> CollisionTiles { get; set; } = new List<ColliisionTileResponse>();

        public ICollection<GameCharacterResponse> GameCharacters { get; set; } = new List<GameCharacterResponse>();
    }
}
