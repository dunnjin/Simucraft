using System;

namespace Simucraft.Client.Common
{
    public class SubscriptionException : Exception
    {
        public SubscriptionException(string message) 
            : base(message)
        {
        }
    }
}
