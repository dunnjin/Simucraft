using Simucraft.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class Game
    {
        public Guid Id { get; set; }

        [RequiredGuid]
        public Guid RulesetId { get; set; }

        public Guid? MapId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();

        public GameStateMode GameState { get; set; }

        public IList<CollisionTile> Tiles { get; set; } = new List<CollisionTile>();

        public int MovementOffset { get; set; }

        public string TurnOrderFormula { get; set; }

        public bool IsOwner { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public ICollection<GameNote> Notes { get; set; } = new List<GameNote>();

        public static Game Empty =>
            new Game
            {

            };
    }
}
