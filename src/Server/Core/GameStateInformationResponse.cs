using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameStateInformationResponse
    {
        public Guid Id { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameStateMode { get; set; }

        public bool IsLocked { get; set; }

        public ICollection<GameCharacterResponse> GameCharacters { get; set; } = new List<GameCharacterResponse>();

        public ICollection<SystemMessage> SystemMessages { get; set; } = new List<SystemMessage>();

        public ICollection<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
    }
}
