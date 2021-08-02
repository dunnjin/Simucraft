using System;

namespace Simucraft.Server.Core
{
    public class GameRequest
    {
        public Guid RulesetId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
