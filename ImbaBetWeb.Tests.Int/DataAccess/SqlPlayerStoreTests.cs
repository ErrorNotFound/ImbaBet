using Allure.NUnit;
using ImbaBetWeb.DataAccess.Stores;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlPlayerStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var player = new Player()
            {
                Id = 1,
                MemberOfCommunityId = 2,
                Points = 3,
                ProfilePicturePath = "Hello"
            };

            var store = new SqlPlayerStore(string.Empty);

            // Act
            var parameters = store.GetParameters(player);
            var map = store.GetPropertyMap();
            var newPlayer = new Player();
            
            foreach (var param in parameters)
            {
                map[param.Key](newPlayer, param.Value);
            }

            // Assert
            Assert.That(newPlayer, Is.EqualTo(player));   
        }

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
