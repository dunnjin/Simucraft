using System;

namespace Simucraft.Server.Core
{
    public class CharacterSkillRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Expression { get; set; }
    }
}
