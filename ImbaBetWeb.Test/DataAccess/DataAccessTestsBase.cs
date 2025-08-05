using Allure.NUnit;
using ImbaBetWeb.Test.DataAccess.TestHelper;

namespace ImbaBetWeb.Test.DataAccess
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