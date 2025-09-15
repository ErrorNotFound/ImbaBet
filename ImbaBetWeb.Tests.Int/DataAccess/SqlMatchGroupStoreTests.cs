using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlMatchGroupStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var mg = new MatchGroup()
            {
                Id = 1,
                Name = "Test MatchGroup",
                HasGroupRanking = true,
                StackRank = 1337
            };

            var store = new SqlMatchGroupStore(string.Empty);

            // Act
            var parameters = store.GetParameters(mg);
            var map = store.GetPropertyMap();
            var newMg = new MatchGroup() {Name = string.Empty};

            foreach (var param in parameters)
            {
                map[param.Key](newMg, param.Value);
            }

            // Assert
            Assert.That(newMg, Is.EqualTo(mg));
        }

        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlMatchGroupStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlMatchGroupStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var matchGroup = new MatchGroup()
            {
                Name = "Match Group Name",
                HasGroupRanking = true,
                StackRank = 1337
            };

            // Test Create and Retrieve
            matchGroup.Id = await store.CreateAsync(matchGroup);
            var retrieved = await store.GetAsync(matchGroup.Id);
            Assert.That(retrieved, Is.EqualTo(matchGroup));

            // Test Update
            matchGroup.Name = "new name";
            matchGroup.HasGroupRanking = false;
            matchGroup.StackRank = 1;
            await store.UpdateAsync(matchGroup);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(matchGroup));

            // Test Delete
            await store.DeleteAsync(matchGroup);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(matchGroup.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
