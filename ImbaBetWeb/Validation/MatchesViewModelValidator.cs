using FluentValidation;
using ImbaBetWeb.ViewModels.Admin;

namespace ImbaBetWeb.Validation
{
    public class MatchesViewModelValidator : AbstractValidator<MatchesViewModel>
    {
        public MatchesViewModelValidator() 
        {
            RuleFor(x => x.Matches).NotEmpty();
            RuleForEach(x => x.Matches).SetValidator(new MatchValidator());
        }
    }
}
