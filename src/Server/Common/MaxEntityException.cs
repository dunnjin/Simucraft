using System;

namespace Simucraft.Server.Common
{
    public class MaxEntityException : Exception
    {
        public MaxEntityException(string message)
            : base(message)
        {
        }
    }
}
