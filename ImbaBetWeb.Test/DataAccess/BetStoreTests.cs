using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Test.DataAccess.TestHelper;

namespace ImbaBetWeb.Test.DataAccess
{
    [AllureNUnit]
    public class BetStoreTests : DataAccessTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new BetStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new BetStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var bet = new NBet()
            {
                UserId = 1,
                MatchId = 2,
                GoalsA = 3,
                GoalsB = 4,
                Points = 5
            };

            // Test Create and Retrieve
            bet.Id = await store.CreateAsync(bet);
            var retrieved = await store.GetAsync(bet.Id);
            Assert.That(retrieved, Is.EqualTo(bet));

            // Test Update
            bet.Points = 1337;
            await store.UpdateAsync(bet);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(bet));

            // Test Delete
            await store.DeleteAsync(bet);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(bet.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
