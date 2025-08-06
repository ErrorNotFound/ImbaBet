using Allure.NUnit;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Tests.Unit.Validation
{
    [AllureNUnit]
    public class GoalValidatorTests
    {
        private GoalValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new GoalValidator();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void GoalValidator_ShouldPass_WhenGoalsReasonable(int goals)
        {
            // Act
            var result = validator.Validate(goals);

            // Assert
            Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(x => x.ErrorMessage)));
        }

        [TestCase(-1)]
        [TestCase(100)]
        [TestCase(999)]
        public void GoalValidator_ShouldFail_WhenGoalsUnreasonable(int goals)
        {
            // Act
            var result = validator.Validate(goals);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
