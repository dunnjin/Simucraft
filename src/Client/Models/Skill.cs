using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Simucraft.Client.Models
{
    public class Skill
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public ICollection<SkillExpression> Expressions { get; set; } = new List<SkillExpression>();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SkillType SkillType { get; set; }

        public string ResistanceTraits { get; set; }

        public string ImmunityTraits { get; set; }


        public static Skill Empty => new Skill();
    }
}
