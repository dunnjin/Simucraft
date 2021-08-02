using System;
using System.Collections;
using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class RequestMove
    {
        public Guid GameCharacterId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
