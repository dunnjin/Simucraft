using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Game : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

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

        public IList<GameCollisionTile> CollisionTiles { get; set; } = new List<GameCollisionTile>();

        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        // TODO: Need display name in other areas determine if it needs to go here.
        public ICollection<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();

        public ICollection<InvitedUser> InvitedUsers { get; set; } = new List<InvitedUser>();

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();

        public ICollection<GameNote> Notes { get; set; } = new List<GameNote>();
        // TODO: Add a header to notes. (this should mimic word doc explaining campaign)
        //public ICollection<string> Notes { get; set; } = new List<string>();
    }
}
