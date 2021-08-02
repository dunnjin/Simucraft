using System;

namespace Simucraft.Server.Common
{
    public class SubscriptionException : Exception
    {
        public SubscriptionException(string message) 
            : base(message)
        {
        }
    }
}
