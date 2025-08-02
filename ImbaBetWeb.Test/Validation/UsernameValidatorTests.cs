using Allure.NUnit;
using ImbaBetWeb.Validation;

namespace ImbaBetWeb.Test.Validation
{
    [AllureNUnit]
    public class UsernameValidatorTests
    {
        private UsernameValidator validator;
        private readonly List<string> existingNames =
            [
                "NameA",
                "NameB",
                "NameC"
            ];


        [SetUp]
        public void Setup()
        {
            validator = new UsernameValidator(existingNames);
        }

        [TestCase("abc")]
        public void UsernameValidator_ShouldPass_WhenGivenNameReasonable(string nameUnderTest)
        {
            // Act
            var result = validator.Validate(nameUnderTest);

            // Assert
            Assert.That(result.IsValid, Is.True, string.Join(";", result.Errors.Select(x => x.ErrorMessage)));
        }

        [TestCase(null)]
        [TestCase("")] // empty
        [TestCase("ab")] // length under 3
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // length over 30
        public void UsernameValidator_ShouldFail_WhenGivenNameUnreasonable(string? nameUnderTest)
        {
            // Act
            var result = validator.Validate(nameUnderTest);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UsernameValidator_ShouldFail_WhenGivenNameIsAlreadyUsed()
        {
            // Act
            var result = validator.Validate(existingNames.First());

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
