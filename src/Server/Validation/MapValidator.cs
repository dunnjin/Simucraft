using FluentValidation;
using Simucraft.Server.Models;

namespace Simucraft.Server.Validation
{
    public class MapValidator : AbstractValidator<Map>
    {
        public MapValidator()
        {
            base.RuleFor(m => m.Name)
                .NotEmpty()
                .MaximumLength(50);

            base.RuleFor(m => m.Description)
                .MaximumLength(250);
        }
    }
}
