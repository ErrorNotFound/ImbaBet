using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class BettingUserStoreTests : DataAccessTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new BettingUserStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new BettingUserStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var user = new BettingUser()
            {
                MemberOfCommunityId = 1,
                Points = 2
            };

            // Test Create and Retrieve
            user.Id = await store.CreateAsync(user);
            var retrieved = await store.GetAsync(user.Id);
            Assert.That(retrieved, Is.EqualTo(user));

            // Test Update
            user.MemberOfCommunityId = 3;
            user.Points = 4;
            await store.UpdateAsync(user);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(user));

            // Test Delete
            await store.DeleteAsync(user);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(user.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task CreateGetUpdate_WithNullables_NoError()
        {
            // Arrange
            var store = new BettingUserStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var user = new BettingUser()
            {
                MemberOfCommunityId = null
            };

            // Test Create and Retrieve
            user.Id = await store.CreateAsync(user);
            var retrieved = await store.GetAsync(user.Id);
            Assert.That(retrieved, Is.EqualTo(user));

            // Test Update
            user.MemberOfCommunityId = null;
            await store.UpdateAsync(user);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(user));
        }
    }
}
