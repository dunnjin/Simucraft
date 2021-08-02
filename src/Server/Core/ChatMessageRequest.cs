using System;

namespace Simucraft.Server.Core
{
    public class ChatMessageRequest
    {
        public string Message { get; set; }

        public bool IsSystem { get; set; }

        public string Tooltip { get; set; }
    }
}
