﻿using Microsoft.EntityFrameworkCore;

namespace Simucraft.Server.Models
{
    [Owned]
    public class GameCollisionTile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public CollisionType CollisionType { get; set; }
    }
}
