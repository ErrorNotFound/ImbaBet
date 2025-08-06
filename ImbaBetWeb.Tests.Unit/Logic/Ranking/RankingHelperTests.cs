using Allure.NUnit;
using ImbaBetWeb.Logic.Ranking;
using Moq;

namespace ImbaBetWeb.Tests.Unit.Logic.Ranking
{
    [AllureNUnit]
    public class RankingHelperTests
    {

        [Test]
        public void SortDescendingAndSetRanks_GivenComparerIsUsed()
        {
            // Arrange
            var list = new List<RankingItem<string>>()
            {
                new() {Details = string.Empty},
                new() {Details = string.Empty}
            };
            var comparer = new Mock<IComparer<RankingItem<string>>>();
            comparer.Setup(x => x.Compare(It.IsAny<RankingItem<string>>(), It.IsAny<RankingItem<string>>())).Returns(0);

            // Act
            RankingHelper.SortDescendingAndSetRanks(list, comparer.Object);

            // Assert
            comparer.Verify(x => x.Compare(It.IsAny<RankingItem<string>>(), It.IsAny<RankingItem<string>>()), Times.AtLeastOnce());
        }

        [Test]
        public void SortDescendingAndSetRanks_TwoIdenticalRankingItems_IdenticalRankIsGiven()
        {
            // Arrange
            var list = new List<RankingItem<string>>()
            {
                new() {Details = string.Empty},
                new() {Details = string.Empty}
            };
            var comparer = new Mock<IComparer<RankingItem<string>>>();
            comparer.Setup(x => x.Compare(It.IsAny<RankingItem<string>>(), It.IsAny<RankingItem<string>>())).Returns(0);

            // Act
            RankingHelper.SortDescendingAndSetRanks(list, comparer.Object);

            // Assert
            Assert.That(list.First().Rank, Is.EqualTo(list.Last().Rank));
        }

        [Test]
        public void SortDescendingAndSetRanks_FirstItem_HasRankOfOne()
        {
            // Arrange
            var list = new List<RankingItem<string>>()
            {
                new() {Details = string.Empty}
            };
            var comparer = new Mock<IComparer<RankingItem<string>>>();

            // Act
            RankingHelper.SortDescendingAndSetRanks(list, comparer.Object);

            // Assert
            Assert.That(list.First().Rank, Is.EqualTo(1));
        }

        [Test]
        public void SortDescendingAndSetRanks_NonEqualItems_SortedWithIncreasingRanks()
        {
            // Arrange
            var list = new List<RankingItem<string>>()
            {
                new() {Details = "B"},
                new() {Details = "C"},
                new() {Details = "A"}
            };
            var comparer = new Mock<IComparer<RankingItem<string>>>();
            comparer.Setup(x => x.Compare(It.IsAny<RankingItem<string>>(), It.IsAny<RankingItem<string>>()))
                .Returns((RankingItem<string> a, RankingItem<string> b) => { return string.Compare(a.Details, b.Details); });

            // Act
            RankingHelper.SortDescendingAndSetRanks(list, comparer.Object);

            // Assert
            for (int i = 0; i < list.Count; i++)
            {
                if(i < list.Count - 2)
                {
                    Assert.That(list[i].Details, Is.GreaterThan(list[i + 1].Details));
                }

                Assert.That(list[i].Rank, Is.EqualTo(i+1));
            }
        }
    }
}
