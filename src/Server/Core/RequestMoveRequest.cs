using System;

namespace Simucraft.Server.Core
{
    public class RequestMoveRequest
    {
        public Guid GameCharacterId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
