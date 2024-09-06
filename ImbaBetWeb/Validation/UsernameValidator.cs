using FluentValidation;
using FluentValidation.Results;

namespace ImbaBetWeb.Validation
{
    public class UsernameValidator : AbstractValidator<string>
    {
        public UsernameValidator(IEnumerable<string> exisitingUsernames) 
        {
            RuleFor(x => x).Cascade(CascadeMode.Stop).NotNull().NotEmpty().Length(3, 30).OverridePropertyName("Username");
            RuleFor(x => x).Must(x => !exisitingUsernames.Any(name => name == x)).WithMessage("Username already existing.");
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Username must not be empty."));
                return false;
            }
            return true;
        }

    }
}
