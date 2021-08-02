using FluentValidation;
using Simucraft.Server.Core;

namespace Simucraft.Server.Validation
{
    public static class FluentValidationHelpers
    {
        public static IRuleBuilderOptions<T, string> NumberExpression<T>(this IRuleBuilder<T, string> ruleBuilder) =>
            ruleBuilder
                .Must(r => r.IsNumberExpression())
                .WithMessage("{PropertyName} must result in a valid number.");
    }
}
