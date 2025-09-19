using Allure.NUnit;
using ImbaBetWeb.DataAccess;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Moq;

namespace ImbaBetWeb.Tests.Unit.DataAccess
{
    [AllureNUnit]
    public class DataStoreManagerTests
    {
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [Test]
        public async Task GetAllSettingsAsync_SettingFromStoreIsProvided()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; });
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);

            // Act
            var settings = await dataStoreManager.GetSettingsAsync();

            // Assert
            Assert.That(settings.Single(), Is.EqualTo(setting));
        }

        [Test]
        public async Task GetCachedSettingValueAsync_SettingAvailableInCache_CacheIsUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; });
            settingStoreMock.Setup(mock => mock.GetAsync(It.IsAny<string>()));
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);
            await dataStoreManager.GetSettingsAsync(); // preload cache    

            // Act
            var settingValue = await dataStoreManager.GetCachedSettingValueAsync<string>(setting.Id);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            settingStoreMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetCachedSettingValueAsync_SettingNotAvailableInCache_StoreIsUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Id))).ReturnsAsync(() => { return setting; });
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);

            // Act
            var settingValue = await dataStoreManager.GetCachedSettingValueAsync<string>(setting.Id);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            settingStoreMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetSettingValueAsync_SettingAvailableInCache_StoreIsAlwaysUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; });
            settingStoreMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Id))).ReturnsAsync(() => { return setting; });
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);
            await dataStoreManager.GetSettingsAsync(); // preload cache 

            // Act
            var settingValue = await dataStoreManager.GetSettingValueAsync<string>(setting.Id);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            settingStoreMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task SetSettingValueAsync_ValueIsConvertedAndSavedInStore()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; });
            settingStoreMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Id))).ReturnsAsync(() => { return setting; });
            settingStoreMock.Setup(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Id == setting.Id)));
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);
            await dataStoreManager.GetSettingsAsync(); // preload cache 
            var newValue = 1337;

            // Act
            await dataStoreManager.SetSettingValueAsync(setting.Id, newValue);

            // Assert
            settingStoreMock.Verify(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Id == setting.Id && set.Value == newValue.ToString())), Times.Once);
        }

        [Test]
        public async Task ResetSettingAsync_ValueIsSetToDefaultValue()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var settingStoreMock = GetSettingsStoreMock();
            settingStoreMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Id))).ReturnsAsync(() => { return setting; });
            settingStoreMock.Setup(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Id == setting.Id && set.Value == setting.Default)));
            var dataStoreManager = new DataStoreManager(null, null, null, null, null, settingStoreMock.Object, null, null, null);

            // Act
            await dataStoreManager.ResetSettingAsync(setting.Id);

            // Assert
            settingStoreMock.Verify(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Id == setting.Id && set.Value == setting.Default)), Times.Once);
        }

        #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        private Mock<ISettingStore> GetSettingsStoreMock()
        {
            var storeMock = new Mock<ISettingStore>();
            storeMock.Setup(mock => mock.EnsureInitializedAsync());
            return storeMock;
        }

        private Setting GetDefaultSetting()
        {
            return new Setting() { Id = "key", Value = "value", Default = "default", Description = "description" };
        }
    }
}
