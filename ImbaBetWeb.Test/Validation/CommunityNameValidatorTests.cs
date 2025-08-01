using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Test.Validation
{
    public class CommunityNameValidatorTests
    {
        private CommunityNameValidator validator;

        [SetUp]
        public void Setup()
        {
            var existingNames = new List<string>()
            {
                "NameA",
                "NameB",
                "NameC"
            };

            validator = new CommunityNameValidator(existingNames);
        }

        [TestCase("abc")]
        public void CommunityNameValidator_ShouldPass_WhenGoalCountReasonable(string nameUnderTest)
        {
            // Act
            var result = validator.Validate(nameUnderTest);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("")] // empty
        [TestCase("ab")] // length under 3
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length over 30
        public void CommunityNameValidator_ShouldFail_WhenGivenNameUnreasonable(string nameUnderTest)
        {
            // Act
            var result = validator.Validate(nameUnderTest);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void CommunityNameValidator_ShouldFail_WhenGivenNameAlreadyUsed()
        {
            // Act
            var result = validator.Validate("NameA");

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
