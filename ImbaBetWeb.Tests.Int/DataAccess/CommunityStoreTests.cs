using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class CommunityStoreTests : DataAccessTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new CommunityStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new CommunityStore(TestDatabase.ConnectionString);
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
