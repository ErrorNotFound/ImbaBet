using FluentValidation;
using ImbaBetWeb.ViewModels.Home;

namespace ImbaBetWeb.Validation
{
    public class ContactViewModelValidator : AbstractValidator<ContactViewModel>
    {
        public ContactViewModelValidator() 
        { 
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
            RuleFor(x => x.EMail).Cascade(CascadeMode.Stop).NotNull().NotEmpty().EmailAddress();
            RuleFor(x => x.Subject).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
            RuleFor(x => x.Message).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
        }
    }
}
