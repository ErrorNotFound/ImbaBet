using FluentValidation;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.Validation
{
    public class BetValidator : AbstractValidator<Bet>
    {
        public BetValidator()
        {
            var goalValidator = new GoalValidator();
            RuleFor(x => x.GoalsA).SetValidator(goalValidator);
            RuleFor(x => x.GoalsB).SetValidator(goalValidator);
        }
    }
}
