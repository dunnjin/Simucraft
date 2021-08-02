using FluentValidation;
using Simucraft.Server.Core;

namespace Simucraft.Server.Validation
{
    public class RulesetRequestValidator : AbstractValidator<RulesetRequest>
    {
        public RulesetRequestValidator()
        {
            base.RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);
            base.RuleFor(r => r.Description)
                .NotEmpty()
                .MaximumLength(1000);
            base.RuleFor(r => r.TurnOrderExpression)
                .NotEmpty()
                .MaximumLength(200);
            base.RuleFor(r => r.MovementOffset)
                .Must(i => i == 2 || i == 3 || i == 4)
                .WithMessage("MovementOffset must be 2, 3, or 4.");
        }
    }
}
