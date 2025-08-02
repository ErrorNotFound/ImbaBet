using FluentValidation;

namespace ImbaBetWeb.Validation
{
    public class GoalValidator : AbstractValidator<int>
    {
        public GoalValidator()
        {
            RuleFor(x => x).InclusiveBetween(0, 99).OverridePropertyName("Goal");
        }
    }
}
