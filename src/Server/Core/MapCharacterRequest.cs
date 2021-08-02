using System;

namespace Simucraft.Server.Core
{
    public class MapCharacterRequest
    {
        public Guid CharacterId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
