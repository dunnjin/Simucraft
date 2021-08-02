using System;

namespace Simucraft.Server.Core
{
    public class ChatMessageResponse : ChatMessageRequest
    {
        public string DisplayName { get; set; }

        public Guid UserId { get; set; }
    }
}
