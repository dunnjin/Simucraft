using Microsoft.EntityFrameworkCore;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class MapCharacter
    {
        public Guid CharacterId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
