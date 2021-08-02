using Simucraft.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace Simucraft.Client.Core
{
    public class NumberExpressionAttribute : ValidationAttribute
    {
        public NumberExpressionAttribute()
            : base("The {0} field must result in a valid number.")
        {
        }

        public override bool IsValid(object value)
        {
            var expression = value as string;
            if (string.IsNullOrEmpty(expression))
                return true;

            return expression.IsNumberExpression();
        }
    }
}
