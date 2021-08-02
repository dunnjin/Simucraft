using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameResponse : GameRequest
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? MapId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public string ImageUrl { get; set; }

        public bool IsOwner { get; set; }

        public int MovementOffset { get; set; }

        public string TurnOrderFormula { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public ICollection<AuthorizedUserResponse> AuthorizedUsers { get; set; } = new List<AuthorizedUserResponse>();

        public ICollection<GameCharacterResponse> GameCharacters { get; set; } = new List<GameCharacterResponse>();

        public IList<ColliisionTileResponse> CollisionTiles { get; set; } = new List<ColliisionTileResponse>();

        public ICollection<GameNoteResponse> Notes { get; set; } = new List<GameNoteResponse>();
    }
}
