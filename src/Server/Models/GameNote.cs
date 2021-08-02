using Microsoft.EntityFrameworkCore;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class GameNote
    {
        public Guid Id { get; set; }

        public string Header { get; set; }

        public string Message { get; set; }
    }
}
