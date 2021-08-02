using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;

namespace Simucraft.Server.Models
{
    [Owned]
    public class CharacterSkill
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Expression { get; set; }
    }
}
