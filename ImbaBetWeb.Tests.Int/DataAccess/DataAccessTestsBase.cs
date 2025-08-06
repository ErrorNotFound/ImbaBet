using Allure.NUnit;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class DataAccessTestsBase
    {
        protected readonly SqlTestDatabase TestDatabase = new();

        [SetUp]
        public async Task SetupAsync()
        {
            await TestDatabase.PrepareAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestDatabase.FinalizeAsync();
        }
    }
}