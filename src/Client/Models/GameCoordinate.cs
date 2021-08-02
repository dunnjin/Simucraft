using System;

namespace Simucraft.Client.Models
{
    public class GameCoordinate
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Guid? GameCharacterId { get; set; }
    }
}
