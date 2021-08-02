using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Common
{
    public class IntegerAttribute : ValidationAttribute
    {
        public IntegerAttribute()
            : base ("The {0} field cannot contain decimals.")
        {
        }

        public override bool IsValid(object value)
        {
            var integer = value as string;
            if (string.IsNullOrEmpty(integer))
                return true;

            return Int32.TryParse(integer, out var _);
        }
    }
}
