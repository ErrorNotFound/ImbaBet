using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlTeamStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlTeamStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlTeamStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var team = new Team()
            {
                Name = "name",
                FlagCountryCode = null,
                StackRank = 1
            };

            // Test Create and Retrieve
            team.Id = await store.CreateAsync(team);
            var retrieved = await store.GetAsync(team.Id);
            Assert.That(retrieved, Is.EqualTo(team));

            // Test Update
            team.Name = "new name";
            team.FlagCountryCode = "de";
            team.StackRank = 2;
            await store.UpdateAsync(team);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(team));

            // Test Delete
            await store.DeleteAsync(team);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(team.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task CreateGetUpdate_WithNullables_NoError()
        {
            // Arrange
            var store = new SqlTeamStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var team = new Team()
            {
                Name = "name",
                FlagCountryCode = null
            };

            // Test Create and Retrieve
            team.Id = await store.CreateAsync(team);
            var retrieved = await store.GetAsync(team.Id);
            Assert.That(retrieved, Is.EqualTo(team));

            // Test Update
            team.FlagCountryCode = null;
            await store.UpdateAsync(team);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(team));
        }
    }
}
