using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameStateInformationRequest
    {
        public Guid Id { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameStateMode { get; set; }

        public bool IsLocked { get; set; }

        public ICollection<GameCharacterRequest> GameCharacters { get; set; } = new List<GameCharacterRequest>();
    }
}
