using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlPlayerStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlPlayerStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlPlayerStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var player = new Player()
            {
                MemberOfCommunityId = 1,
                Points = 2,
                ProfilePicturePath = "path"
            };

            // Test Create and Retrieve
            player.Id = await store.CreateAsync(player);
            var retrieved = await store.GetAsync(player.Id);
            Assert.That(retrieved, Is.EqualTo(player));

            // Test Update
            player.MemberOfCommunityId = 3;
            player.Points = 4;
            player.ProfilePicturePath = "newPath";
            await store.UpdateAsync(player);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(player));

            // Test Delete
            await store.DeleteAsync(player);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(player.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task CreateGetUpdate_WithNullables_NoError()
        {
            // Arrange
            var store = new SqlPlayerStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var player = new Player()
            {
                MemberOfCommunityId = null,
                ProfilePicturePath = null
            };

            // Test Create and Retrieve
            player.Id = await store.CreateAsync(player);
            var retrieved = await store.GetAsync(player.Id);
            Assert.That(retrieved, Is.EqualTo(player));

            // Test Update
            player.MemberOfCommunityId = null;
            player.ProfilePicturePath= null;
            await store.UpdateAsync(player);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(player));
        }
    }
}
