using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class GameStateInformation
    {
        public Guid Id { get; set; }

        public Guid? CurrentTurnId { get; set; }

        public GameStateMode GameStateMode { get; set; }

        public bool IsLocked { get; set; }

        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();

        public ICollection<Coordinate> Coordinates { get; set; } = new List<Coordinate>();

        public static GameStateInformation Empty =>
            new GameStateInformation
            {

            };
    }
}
