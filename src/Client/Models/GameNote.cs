using System;

namespace Simucraft.Client.Models
{
    public class GameNote
    {
        public Guid Id { get; set; }

        public string Header { get; set; }

        public string Message { get; set; }

        public static GameNote Empty =>
            new GameNote
            {
            };
    }
}
