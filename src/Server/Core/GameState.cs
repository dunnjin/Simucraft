using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameState
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? MapId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public string ImageUrl { get; set; }

        public int MovementOffset { get; set; }

        public string TurnOrderFormula { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameStateMode { get; set; }

        public bool IsLocked { get; set; }

        public IList<ColliisionTileResponse> Tiles { get; set; } = new List<ColliisionTileResponse>();

        // TODO: Need display name in other areas determine if it needs to go here.
        public ICollection<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();
        //public ICollection<Guid> InvitedUserIds { get; set; } = new List<Guid>();

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();

        public ICollection<GameNote> Notes { get; set; } = new List<GameNote>();
    }
}
