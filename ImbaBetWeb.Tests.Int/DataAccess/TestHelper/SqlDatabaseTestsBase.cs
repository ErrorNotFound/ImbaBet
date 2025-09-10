namespace ImbaBetWeb.Tests.Int.DataAccess.TestHelper
{
    public class SqlDatabaseTestsBase
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