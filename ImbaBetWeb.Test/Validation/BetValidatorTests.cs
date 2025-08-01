using Allure.NUnit;
using ImbaBetWeb.Models;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Test.Validation
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

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(0, 5)]
        [TestCase(5, 0)]
        [TestCase(10, 10)]
        public void BetValidator_ShouldPass_WhenGoalCountReasonable(int goalA, int goalB)
        {
            // Arrange
            var bet = new Bet() { UserId = string.Empty, GoalsA = goalA, GoalsB = goalB };

            // Act
            var result = validator.Validate(bet);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(999, 0)]
        [TestCase(0, 999)]
        public void BetValidator_ShouldFail_WhenGoalCountUnreasonable(int goalA, int goalB)
        {
            // Arrange
            var bet = new Bet() { UserId = string.Empty, GoalsA = goalA, GoalsB = goalB };

            // Act
            var result = validator.Validate(bet);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
