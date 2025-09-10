using Allure.NUnit;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Tests.Unit.Validation
{
    [AllureNUnit]
    public class CommunityNameValidatorTests
    {
        private CommunityNameValidator validator;
        private readonly List<string> existingNames =
            [
                "NameA",
                "NameB",
                "NameC"
            ];


        [SetUp]
        public void Setup()
        {
            validator = new CommunityNameValidator(existingNames);
        }

        [TestCase("abc")]
        public void CommunityNameValidator_ShouldPass_WhenGivenNameReasonable(string nameUnderTest)
        {
            // Act
            var result = validator.Validate(nameUnderTest);

            // Assert
            Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(x => x.ErrorMessage)));
        }

        [TestCase(null)] // empty
        [TestCase("")] // empty
        [TestCase("ab")] // length under 3
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length over 30
        public void CommunityNameValidator_ShouldFail_WhenGivenNameUnreasonable(string? nameUnderTest)
        {
            // Act
            #pragma warning disable CS8604 // Possible null reference argument.
            var result = validator.Validate(nameUnderTest);
            #pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void CommunityNameValidator_ShouldFail_WhenGivenNameIsAlreadyUsed()
        {
            // Act
            var result = validator.Validate(existingNames.First());

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
