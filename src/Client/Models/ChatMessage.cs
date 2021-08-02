using System;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class ChatMessage
    {
        public Guid UserId { get; set; }

        public string DisplayName { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public bool IsSystem { get; set; }

        public string Tooltip { get; set; }

        public static ChatMessage Empty =>
            new ChatMessage
            {

            };
    }
}
