using System;

namespace Simucraft.Server.Core
{
    public class GameCharacterSkillRequest
    {
        public Guid SkillId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Expression { get; set; }
    }
}
