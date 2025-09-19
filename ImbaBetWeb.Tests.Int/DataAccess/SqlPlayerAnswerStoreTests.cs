using Allure.NUnit;
using ImbaBetWeb.DataAccess.Stores;
using ImbaBetWeb.Model.Questions;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlPlayerAnswerStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var playerAnswer = new PlayerAnswer()
            {
                Id = 1,
                QuestionId = 2,
                PlayerId = 3,
                Answer = "An answer"
            };

            var store = new SqlPlayerAnswerStore(string.Empty);

            // Act
            var parameters = store.GetParameters(playerAnswer);
            var map = store.GetPropertyMap();
            var newPlayerAnswer = new PlayerAnswer() { Answer = string.Empty };

            foreach (var param in parameters)
            {
                map[param.Key](newPlayerAnswer, param.Value);
            }

            // Assert
            Assert.That(newPlayerAnswer, Is.EqualTo(playerAnswer));
        }

        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlPlayerAnswerStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlPlayerAnswerStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var playerAnswer = new PlayerAnswer()
            {
                QuestionId = 2,
                PlayerId = 3,
                Answer = "An answer"
            };

            // Test Create and Retrieve
            playerAnswer.Id = await store.CreateAsync(playerAnswer);
            var retrieved = await store.GetAsync(playerAnswer.Id);
            Assert.That(retrieved, Is.EqualTo(playerAnswer));

            // Test Update
            playerAnswer.QuestionId = 4;
            playerAnswer.PlayerId = 5;
            playerAnswer.Answer = "new answer";

            await store.UpdateAsync(playerAnswer);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(playerAnswer));

            // Test Delete
            await store.DeleteAsync(playerAnswer);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(playerAnswer.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
