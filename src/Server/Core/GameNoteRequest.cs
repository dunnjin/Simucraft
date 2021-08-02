using System;

namespace Simucraft.Server.Core
{
    public class GameNoteRequest
    {
        public Guid Id { get; set; }

        public string Header { get; set; }

        public string Message { get; set; }
    }
}
