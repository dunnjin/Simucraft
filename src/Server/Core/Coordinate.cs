using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Core
{
    public class Coordinate
    {
        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
