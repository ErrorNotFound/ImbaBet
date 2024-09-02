using FluentValidation;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.Validation
{
    public class BetValidator : AbstractValidator<Bet>
    {
        public BetValidator()
        {
            RuleFor(x => x.GoalsA).InclusiveBetween(0, 99).OverridePropertyName("Goal");
            RuleFor(x => x.GoalsB).InclusiveBetween(0, 99).OverridePropertyName("Goal");
        }
    }
}
