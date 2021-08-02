using Microsoft.EntityFrameworkCore;

namespace Simucraft.Server.Models
{
    [Owned]
    public class EquipmentExpression
    {
        public string SelfExpression { get; set; }

        public string TargetExpression { get; set; }

        public string OperatorExpression { get; set; }
    }
}
