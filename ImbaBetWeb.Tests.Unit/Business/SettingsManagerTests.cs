using Allure.NUnit;
using ImbaBetWeb.Business;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Moq;

namespace ImbaBetWeb.Tests.Unit.Business
{
    [AllureNUnit]
    public class SettingsManagerTests
    {
        [Test]
        public void Constructor_StoreIsInitialized()
        {
            // Arrange
            var storeMock = GetStoreMock();

            // Act
            var settingsManager = new SettingsManager(storeMock.Object);

            // Assert
            storeMock.Verify(mock => mock.EnsureInitializedAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllSettingsAsync_SettingFromStoreIsProvided()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; });
            var settingsManager = new SettingsManager(storeMock.Object);

            // Act
            var settings = await settingsManager.GetAllSettingsAsync();

            // Assert
            Assert.That(settings.Single(), Is.EqualTo(setting));
        }

        [Test]
        public async Task GetCachedSettingValueAsync_SettingAvailableInCache_CacheIsUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; }); // this will make sure the setting is in cache
            storeMock.Setup(mock => mock.GetAsync(It.IsAny<string>()));
            var settingsManager = new SettingsManager(storeMock.Object);

            // Act
            var settingValue = await settingsManager.GetCachedSettingValueAsync<string>(setting.Key);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            storeMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetCachedSettingValueAsync_SettingNotAvailableInCache_StoreIsUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return []; }); // setting will not be in cache
            storeMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Key))).ReturnsAsync(() => { return setting; });
            var settingsManager = new SettingsManager(storeMock.Object);

            // Act
            var settingValue = await settingsManager.GetCachedSettingValueAsync<string>(setting.Key);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            storeMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetSettingValueAsync_SettingAvailableInCache_StoreIsAlwaysUsed()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; }); // this will make sure the setting is in cache
            storeMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Key))).ReturnsAsync(() => { return setting; });
            var settingsManager = new SettingsManager(storeMock.Object);

            // Act
            var settingValue = await settingsManager.GetSettingValueAsync<string>(setting.Key);

            // Assert
            Assert.That(settingValue, Is.EqualTo(setting.Value));
            storeMock.Verify(mock => mock.GetAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task SetSettingValueAsync_ValueIsConvertedAndSavedInStore()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAllAsync()).ReturnsAsync(() => { return [setting]; }); // this will make sure the setting is in cache
            storeMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Key))).ReturnsAsync(() => { return setting; });
            storeMock.Setup(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Key == setting.Key)));
            var settingsManager = new SettingsManager(storeMock.Object);
            var newValue = 1337;

            // Act
            await settingsManager.SetSettingValueAsync(setting.Key, newValue);

            // Assert
            storeMock.Verify(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Key == setting.Key && set.Value == newValue.ToString())), Times.Once);
        }

        [Test]
        public async Task ResetSettingAsync_ValueIsSetToDefaultValue()
        {
            // Arrange
            var setting = GetDefaultSetting();
            var storeMock = GetStoreMock();
            storeMock.Setup(mock => mock.GetAsync(It.Is<string>((arg) => arg == setting.Key))).ReturnsAsync(() => { return setting; });
            storeMock.Setup(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Key == setting.Key && set.Value == setting.Default)));
            var settingsManager = new SettingsManager(storeMock.Object);

            // Act
            await settingsManager.ResetSettingAsync(setting.Key);

            // Assert
            storeMock.Verify(mock => mock.UpdateAsync(It.Is<Setting>((set) => set.Key == setting.Key && set.Value == setting.Default)), Times.Once);
        }

        private Mock<ISettingStore> GetStoreMock()
        {
            var storeMock = new Mock<ISettingStore>();
            storeMock.Setup(mock => mock.EnsureInitializedAsync());
            return storeMock;
        }

        private Setting GetDefaultSetting()
        {
            return new Setting() { Key = "key", Value = "value", Default = "default", Description = "description" };
        }
    }
}
