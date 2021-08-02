using System;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class GameCharactersResponse
    {
        public int NewGameCharactersCount { get; set; }

        public GameStateMode GameState { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();

        public ICollection<GameNote> Notes { get; set; } = new List<GameNote>();
    }
}
