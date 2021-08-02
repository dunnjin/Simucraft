using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class SkillResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ResistanceTraits { get; set; }

        public string ImmunityTraits { get; set; }

        public string SkillType { get; set; }

        public ICollection<SkillExpressionResponse> Expressions { get; set; } = new List<SkillExpressionResponse>();
    }
}
