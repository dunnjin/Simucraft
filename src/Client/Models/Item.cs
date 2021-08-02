using System;

namespace Simucraft.Client.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Modifier { get; set; }
    }
}
