using Allure.NUnit;
using FluentValidation.TestHelper;
using ImbaBetWeb.Model;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Tests.Unit.Validation
{
    [AllureNUnit]
    public class MatchValidatorTests
    {
        private MatchValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new MatchValidator();
        }

        private Match GetValidMatchObject()
        {
            return new Match() { GoalsA = 0, GoalsB = 0, AlternativeTeamAText = "a", AlternativeTeamBText = "b" };
        }

        [Test]
        public void MatchValidator_ShouldPass_ValidMatchObjectIsValid()
        {
            // Arrange
            var match = GetValidMatchObject();

            // Act
            var result = validator.Validate(match);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void MatchValidator_Goals_ValidatedByGoalValidator()
        {
            validator.ShouldHaveChildValidator(x => x.GoalsA, typeof(GoalValidator));
            validator.ShouldHaveChildValidator(x => x.GoalsB, typeof(GoalValidator));
        }

        [Test]
        public void MatchValidator_ShouldFail_NeitherTeamOrAlternativeTextIsSet()
        {
            // Arrange
            var match = GetValidMatchObject();
            match.TeamA = null;
            match.TeamB = null;
            match.AlternativeTeamAText = null;
            match.AlternativeTeamBText = null;

            // Act
            var result = validator.TestValidate(match);

            // Assert
            Assert.That(result.IsValid, Is.False);
            result.ShouldHaveValidationErrorFor(m => m.AlternativeTeamAText);
            result.ShouldHaveValidationErrorFor(m => m.AlternativeTeamBText);
        }

        [Test]
        public void MatchValidator_ShouldFail_MatchIsOverButHasFutureDate()
        {
            // Arrange
            var match = GetValidMatchObject();
            match.IsOver = true;
            match.DateTime = DateTime.UtcNow + TimeSpan.FromDays(1);

            // Act
            var result = validator.TestValidate(match);

            // Assert
            Assert.That(result.IsValid, Is.False);
            result.ShouldHaveValidationErrorFor(m => m.IsOver);
        }
    }
}
