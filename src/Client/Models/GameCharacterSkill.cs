using System;

namespace Simucraft.Client.Models
{
    public class GameCharacterSkill
    {
        public Guid SkillId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Expression { get; set; }
    }
}
