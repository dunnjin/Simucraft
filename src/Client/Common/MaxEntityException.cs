using System;

namespace Simucraft.Client.Common
{
    public class MaxEntityException : Exception
    {
        public MaxEntityException(string message)
            : base(message)
        {
        }
    }
}
