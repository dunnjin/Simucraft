using Microsoft.EntityFrameworkCore;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class ChatMessage
    {
        public Guid UserId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string DisplayName { get; set; }

        public string Message { get; set; }

        public string Tooltip { get; set; }

        public bool IsSystem { get; set; }
    }
}
