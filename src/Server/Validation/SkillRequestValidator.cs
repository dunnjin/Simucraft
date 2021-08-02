using FluentValidation;
using Simucraft.Server.Core;

namespace Simucraft.Server.Validation
{
    public class SkillRequestValidator : AbstractValidator<SkillRequest>
    {
        public SkillRequestValidator()
        {
            base.RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);

            base.RuleFor(r => r.Category)
                .NotEmpty()
                .MaximumLength(50);

            base.RuleFor(r => r.Description)
                .MaximumLength(1000);

            base.RuleForEach(r => r.Expressions)
                .ChildRules(r =>
                {
                    r.RuleFor(c => c.OperatorExpression)
                     .Must(c => c == "<" || c == ">" || c == "=")
                     .WithMessage("{PropertyName} must have a value of <, >, or =.");

                    r.RuleFor(c => c.SelfExpression)
                     .NumberExpression()
                     .MaximumLength(200);

                    r.RuleFor(c => c.TargetExpression)
                     .NumberExpression()
                     .MaximumLength(200);
                });

            base.RuleFor(r => r.Expressions)
                .Must(r => r.Count <= 5)
                .WithMessage("Skill cannot exceed 5 expressions.");
        }
    }
}
