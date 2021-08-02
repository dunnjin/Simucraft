using System;

namespace Simucraft.Client.Models
{
    public class CharacterSkill
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Expression { get; set; }

        public static CharacterSkill Empty =>
            new CharacterSkill
            {
                Id = Guid.NewGuid(),
            };
    }
}
