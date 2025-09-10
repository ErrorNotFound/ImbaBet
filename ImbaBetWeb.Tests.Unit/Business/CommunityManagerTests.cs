using Allure.NUnit;
using ImbaBetWeb.Business;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;
using Moq;

namespace ImbaBetWeb.Tests.Unit.Business
{
    [AllureNUnit]
    public class CommunityManagerTests
    {
        [Test]
        public async Task CreateCommunityAsync_AllValuesAreSetCorrectly()
        {
            // Arrange
            var player = new Player() { Id = 1337, MemberOfCommunityId = null };
            var communityName = "new community";
            var expectedCommunityId = 42;

            var mockDataStoreManager = new Mock<IDataStoreManager>();
            mockDataStoreManager.Setup(m => m.CreateCommunityAsync(It.IsAny<Community>())).ReturnsAsync(expectedCommunityId);
            mockDataStoreManager.Setup(m => m.GetSettingValueAsync<bool>(It.Is<string>(s => s == SettingNames.ALLOW_COMMUNITY_CREATE))).ReturnsAsync(true);
            
            var manager = new CommunityManager(mockDataStoreManager.Object);

            // Act
            await manager.CreateCommunityAsync(player, communityName);

            // Assert
            mockDataStoreManager.Verify(m => m.GetSettingValueAsync<bool>(It.Is<string>(s => s == SettingNames.ALLOW_COMMUNITY_CREATE)), Times.Once);
            mockDataStoreManager.Verify(m => m.CreateCommunityAsync(It.Is<Community>(c => c.Name == communityName && c.OwnerId == player.Id && c.Members.Contains(player))), Times.Once);
            mockDataStoreManager.Verify(m => m.UpdatePlayersAsync(It.Is<IEnumerable<Player>>(players => players.Count() == 1 && players.First().Id == player.Id && players.First().MemberOfCommunityId == expectedCommunityId)), Times.Once);
        }

        [Test]
        public async Task CreateCommunityAsync_CommunityCreationIsNotAllowed_NoCommunityIsCreated()
        {
            // Arrange
            var player = new Player() { Id = 1337, MemberOfCommunityId = null };
            var communityName = "new community";

            var mockDataStoreManager = new Mock<IDataStoreManager>();
            mockDataStoreManager.Setup(m => m.GetSettingValueAsync<bool>(It.Is<string>(s => s == SettingNames.ALLOW_COMMUNITY_CREATE))).ReturnsAsync(false);

            var manager = new CommunityManager(mockDataStoreManager.Object);

            // Act
            await manager.CreateCommunityAsync(player, communityName);

            // Assert
            mockDataStoreManager.Verify(m => m.GetSettingValueAsync<bool>(It.Is<string>(s => s == SettingNames.ALLOW_COMMUNITY_CREATE)), Times.Once);
            mockDataStoreManager.Verify(m => m.CreateCommunityAsync(It.IsAny<Community>()), Times.Never);
        }
    }
}
