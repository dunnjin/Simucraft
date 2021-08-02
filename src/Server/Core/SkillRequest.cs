using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Core
{
    public class SkillRequest
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ResistanceTraits { get; set; }

        public string ImmunityTraits { get; set; }

        public string SkillType { get; set; }

        public ICollection<SkillExpressionRequest> Expressions { get; set; } = new List<SkillExpressionRequest>();
    }
}
