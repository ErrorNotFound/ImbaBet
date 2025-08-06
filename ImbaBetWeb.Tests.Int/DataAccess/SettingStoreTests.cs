using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SettingStoreTests : DataAccessTestsBase
    {
        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SettingStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SettingStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var setting = new Setting()
            {
                Key = "key",
                Value = "value",
                Default = "default",
                Description = "description"
            };

            // Test Create and Retrieve
            await store.CreateAsync(setting);
            var retrieved = await store.GetAsync(setting.Key);
            Assert.That(retrieved, Is.EqualTo(setting));

            // Test Update
            setting.Description = "updated description";
            await store.UpdateAsync(setting);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(setting));

            // Test Delete
            await store.DeleteAsync(setting);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(setting.Key));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
