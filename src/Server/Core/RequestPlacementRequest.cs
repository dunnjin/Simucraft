using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Core
{
    public class RequestPlacementRequest
    {
        public Guid GameCharacterId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
