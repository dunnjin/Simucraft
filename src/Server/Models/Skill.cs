using System;
using System.Collections.Generic;

namespace Simucraft.Server.Models
{
    public class Skill : IAggregateRoot
    {
        public Guid Id { get; set; }

        public Guid RulesetId { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ResistanceTraits { get; set; }

        public string ImmunityTraits { get; set; }

        public SkillType SkillType { get; set; }

        public ICollection<SkillExpression> Expressions { get; set; } = new List<SkillExpression>();
    }
}
