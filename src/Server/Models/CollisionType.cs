using System;

namespace Simucraft.Server.Models
{
    [Flags]
    public enum CollisionType
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
    }
}
