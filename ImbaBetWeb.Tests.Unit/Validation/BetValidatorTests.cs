using Allure.NUnit;
using FluentValidation.TestHelper;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Tests.Unit.Validation
{
    [AllureNUnit]
    public class BetValidatorTests
    {
        private BetValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new BetValidator();
        }

        [Test]
        public void BetValidator_Goals_ValidatedByGoalValidator()
        {
            validator.ShouldHaveChildValidator(x => x.GoalsA, typeof(GoalValidator));
            validator.ShouldHaveChildValidator(x => x.GoalsB, typeof(GoalValidator));
        }
    }
}
