using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.Model;
using ImbaBetWeb.Tests.Int.DataAccess.TestHelper;

namespace ImbaBetWeb.Tests.Int.DataAccess
{
    [AllureNUnit]
    public class SqlSettingStoreTests : SqlDatabaseTestsBase
    {
        [Test]
        public void ParametersAndPropertyMap_ItemIsDuplicated_Identical()
        {
            // Arrange
            var setting = new Setting()
            {
                Id = "Id",
                Value = "Value",
                Default = "Default",
                Description = "Description"
            };

            var store = new SqlSettingStore(string.Empty);

            // Act
            var parameters = store.GetParameters(setting);
            var map = store.GetPropertyMap();
            var newSetting = new Setting() { Id = string.Empty, Value = string.Empty, Default = string.Empty, Description = string.Empty };

            foreach (var param in parameters)
            {
                map[param.Key](newSetting, param.Value);
            }

            // Assert
            Assert.That(newSetting, Is.EqualTo(setting));
        }

        [Test]
        public async Task EnsureInitializedAsync_NoTableAvailable_TableIsAdded()
        {
            // Arrange
            var store = new SqlSettingStore(TestDatabase.ConnectionString);

            // Act and Assert
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.False);
            await store.EnsureInitializedAsync();
            Assert.That(await TestDatabase.TableExistsAsync(store.TableName), Is.True);
        }

        [Test]
        public async Task CreateGetUpdateDelete_TestWorkflow_NoUnexpectedError()
        {
            // Arrange
            var store = new SqlSettingStore(TestDatabase.ConnectionString);
            await store.EnsureInitializedAsync();
            var setting = new Setting()
            {
                Id = "id",
                Value = "value",
                Default = "default",
                Description = "description"
            };

            // Test Create and Retrieve
            await store.CreateAsync(setting);
            var retrieved = await store.GetAsync(setting.Id);
            Assert.That(retrieved, Is.EqualTo(setting));

            // Test Update
            setting.Value = "updated value";
            setting.Default = "updated default";
            setting.Description = "updated description";
            await store.UpdateAsync(setting);
            retrieved = (await store.GetAllAsync()).Single();
            Assert.That(retrieved, Is.EqualTo(setting));

            // Test Delete
            await store.DeleteAsync(setting);
            Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAsync(setting.Id));
            var items = await store.GetAllAsync();
            Assert.That(items, Is.Empty);
        }
    }
}
