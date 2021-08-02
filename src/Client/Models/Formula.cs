using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class Formula
    {
        [StringLength(50)]
        public string Self { get; set; }

        [StringLength(50)]
        public string Target { get; set; }

        public string Operator { get; set; }
    }
}
