using FluentValidation;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.Validation
{
    public class MatchPlanValidator : AbstractValidator<Matchplan>
    {
        public MatchPlanValidator() 
        {
            RuleFor(x => x.Matches).NotEmpty();
            RuleForEach(x => x.Matches).SetValidator(new MatchValidator());
        }
    }
}
