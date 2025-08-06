using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class MatchStoreTests : DataAccessTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new MatchStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new MatchStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var match = new NMatch()
            {
                Id = 1,
                DateTime = DateTime.Now,
                TeamATeamId = 2,
                TeamBTeamId = 3,
                AlternativeTeamAText = "a",
                AlternativeTeamBText =  "b",
                GoalsA = 4,
                GoalsB = 5,
                IsOver = true,
                MatchGroupId = 6
            };

            // Test Create and Retrieve
            match.Id = await store.CreateAsync(match);
            var retrieved = await store.GetAsync(match.Id);
            Assert.That(retrieved, Is.EqualTo(match));

            // Test Update
            match.DateTime = DateTime.Today + TimeSpan.FromDays(1);
            match.TeamATeamId = 12;
            match.TeamATeamId = 13;
            match.AlternativeTeamAText = "a new";
            match.AlternativeTeamBText = "b new";
            match.GoalsA = 14;
            match.GoalsB = 15;
            match.IsOver = false;
            match.MatchGroupId = 16;
            await store.UpdateAsync(match);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(match));

            // Test Delete
            await store.DeleteAsync(match);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(match.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
