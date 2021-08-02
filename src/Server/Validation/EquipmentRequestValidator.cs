using FluentValidation;
using Simucraft.Client.Models;
using Simucraft.Server.Core;
using System;

namespace Simucraft.Server.Validation
{
    public class EquipmentRequestValidator : AbstractValidator<EquipmentRequest>
    {
        public EquipmentRequestValidator()
        {
            base.RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(50);

            base.RuleFor(r => r.Category)
                .NotEmpty()
                .MaximumLength(50);

            base.RuleFor(r => r.Description)
                .MaximumLength(1000);

            base.RuleFor(r => r.EquipmentType)
                .Must(r => Enum.TryParse<EquipmentType>(r, out _))
                .WithMessage("{EquipmentType} invalid value.");

            base.RuleFor(r => r.Range)
                .InclusiveBetween(1, 100);

            base.RuleFor(r => r.Cost)
                .MaximumLength(50);

            base.RuleFor(r => r.Weight)
                .InclusiveBetween(0, 100000);

            base.RuleFor(r => r.DamageTraits)
                .MaximumLength(200);

            base.RuleFor(r => r.ResistanceTraits)
                .MaximumLength(200);

            base.RuleFor(r => r.ImmunityTraits)
                .MaximumLength(200);

            base.RuleFor(r => r.DamageExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.CriticalDamageExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.DamageOperatorExpression)
                .Must(r => r == "+" || r == "*")
                .WithMessage("{PropertyName} must have a value of + or *.");

            base.RuleFor(r => r.HitChanceSelfExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.HitChanceTargetExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.HitChanceOperatorExpression)
                .Must(r => r == ">" || r == "<" || r == "=")
                .WithMessage("{PropertyName} must have a value of >, <, or =.");

            base.RuleFor(r => r.CriticalHitChanceOperatorExpression)
                .Must(r => r == ">" || r == "<" || r == "=")
                .WithMessage("{PropertyName} must have a value of >, <, or =.");

            base.RuleFor(r => r.CriticalHitChanceSelfExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.CriticalHitChanceTargetExpression)
                .NumberExpression()
                .MaximumLength(200);

            base.RuleFor(r => r.PassiveExpressions)
                .Must(r => r.Count <= 5)
                .WithMessage("Equipment cannot exceed 5 expressions.");

            base.RuleForEach(r => r.PassiveExpressions)
                .ChildRules(r =>
                {
                    r.RuleFor(c => c.OperatorExpression)
                     .Must(c => c == "=" || c == "+=" || c == "-=")
                     .WithMessage("{PropertyName} must have a value of =, +=, or -=.");

                    r.RuleFor(c => c.SelfExpression)
                     .NumberExpression()
                     .MaximumLength(200);

                    r.RuleFor(c => c.TargetExpression)
                     .NumberExpression()
                     .MaximumLength(200);
                });
        }
    }
}
