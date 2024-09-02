using FluentValidation;
using FluentValidation.Results;
using System;

namespace ImbaBetWeb.Validation
{
    public class CommunityNameValidator : AbstractValidator<string>
    {
        public CommunityNameValidator(IEnumerable<string> exisitingCommunityNames)
        {
            RuleFor(x => x).NotNull().NotEmpty().Length(3, 30).OverridePropertyName("Community Name");
            RuleFor(x => x).Must(x => !exisitingCommunityNames.Any(name => name == x)).WithMessage("Community name already exisiting.");
        }

        // validate that the model itself is not null
        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Community name must be provided."));
                return false;
            }
            return true;
        }
    }
}
