using Allure.NUnit;
using ImbaBetWeb.DataAccess.Stores;
using ImbaBetWeb.Model.Questions;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlQuestionStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var question = new Question()
            {
                Id = 1,
                Text = "test",
                Type = QuestionType.SingleTeam,
                DueDate = DateTime.Now,
                IsOver = true,
                ChoicesRaw = "1;2;3;4",
                CorrectAnswer = "2",
                Points = 5
            };

            var store = new SqlQuestionStore(string.Empty);

            // Act
            var parameters = store.GetParameters(question);
            var map = store.GetPropertyMap();
            var newQuestion = new Question();

            foreach (var param in parameters)
            {
                map[param.Key](newQuestion, param.Value);
            }

            // Assert
            Assert.That(newQuestion, Is.EqualTo(question));
        }

        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlQuestionStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlQuestionStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var question = new Question()
            {
                Text = "test",
                Type = QuestionType.SingleTeam,
                DueDate = DateTime.Now,
                IsOver = true,
                ChoicesRaw = "1;2;3;4",
                CorrectAnswer = "2",
                Points = 5
            };

            // Test Create and Retrieve
            question.Id = await store.CreateAsync(question);
            var retrieved = await store.GetAsync(question.Id);
            Assert.That(retrieved, Is.EqualTo(question));

            // Test Update
            question.Text = "updated";
            question.Type = QuestionType.Number;
            question.DueDate = question.DueDate.AddDays(1);
            question.IsOver = false;
            question.ChoicesRaw = "5;6;7;8";
            question.CorrectAnswer = "6";
            question.Points = 10;

            await store.UpdateAsync(question);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(question));

            // Test Delete
            await store.DeleteAsync(question);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(question.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task CreateGetUpdate_WithNullables_NoError()
        {
            // Arrange
            var store = new SqlQuestionStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var question = new Question()
            {
                ChoicesRaw = null,
                CorrectAnswer = null
            };

            // Test Create and Retrieve
            question.Id = await store.CreateAsync(question);
            var retrieved = await store.GetAsync(question.Id);
            Assert.That(retrieved, Is.EqualTo(question));

            // Test Update
            question.ChoicesRaw = null;
            question.CorrectAnswer = null;
            await store.UpdateAsync(question);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(question));
        }
    }
}
