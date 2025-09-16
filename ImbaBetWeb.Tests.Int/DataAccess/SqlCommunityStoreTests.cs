using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlCommunityStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var community = new Community()
            {
                Id = 1,
                Name = "Test Community",
                OwnerId = 2
            };

            var store = new SqlCommunityStore(string.Empty);

            // Act
            var parameters = store.GetParameters(community);
            var map = store.GetPropertyMap();
            var newCommunity = new Community() { Name = string.Empty, OwnerId = 0 };

            foreach (var param in parameters)
            {
                map[param.Key](newCommunity, param.Value);
            }

            // Assert
            Assert.That(newCommunity, Is.EqualTo(community));
        }

        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlCommunityStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlCommunityStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var community = new Community()
            {
                Name = "Test Community",
                OwnerId = 1
            };

            // Test Create and Retrieve
            community.Id = await store.CreateAsync(community);
            var retrieved = await store.GetAsync(community.Id);
            Assert.That(retrieved, Is.EqualTo(community));

            // Test Update
            community.Name = "A new name";
            community.OwnerId = 2;
            await store.UpdateAsync(community);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(community));

            // Test Delete
            await store.DeleteAsync(community);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(community.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
