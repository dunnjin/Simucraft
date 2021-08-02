using System;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class GameCharactersRequest
    {
        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameState { get; set; }

        public ICollection<GameNote> Notes { get; set; } = new List<GameNote>();

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();
    }
}
