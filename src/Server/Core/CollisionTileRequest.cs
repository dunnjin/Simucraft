using Simucraft.Server.Models;

namespace Simucraft.Server.Core
{
    public class CollisionTileRequest
    {
        public int X { get; set; }

        public int Y { get; set; }

        public CollisionType CollisionType { get; set; }
    }
}
