using Simucraft.Client.Core;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Models
{
    public class EquipmentExpression
    {
        [StringLength(200)]
        [NumberExpression]
        public string SelfExpression { get; set; }

        [StringLength(200)]
        [NumberExpression]
        public string TargetExpression { get; set; }

        [Required]
        [StringLength(5)]
        public string OperatorExpression { get; set; }

        public static EquipmentExpression Empty =>
            new EquipmentExpression
            {
                OperatorExpression = "=",
            };
    }
}
