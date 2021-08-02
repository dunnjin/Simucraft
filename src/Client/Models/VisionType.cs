using System;

namespace Simucraft.Client.Models
{
    [Flags]
    public enum VisionType
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
    }
}
