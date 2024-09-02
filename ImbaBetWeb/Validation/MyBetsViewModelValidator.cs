using FluentValidation;
using ImbaBetWeb.ViewModels.Betting;

namespace ImbaBetWeb.Validation
{
    public class MyBetsViewModelValidator : AbstractValidator<MyBetsViewModel>
    {
        public MyBetsViewModelValidator() 
        {
            RuleForEach(x => x.OpenBets).SetValidator(new BetValidator());
        }
    }
}
