using Allure.NUnit;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Logic.Ranking.Comparer;
using ImbaBetWeb.Logic.Ranking.Details;

namespace ImbaBetWeb.Test.Logic.Ranking.Comparer
{
    [AllureNUnit]
    public class CommunityComparerTests
    {
        private CommunityComparer comparer;

        [SetUp]
        public void Setup()
        {
            comparer = new CommunityComparer();
        }

        [Test]
        public void Compare_EqualItems_ReturnsEqual()
        {
            // Arrange
            var items = GetEqualItems();

            // Act
            var result = comparer.Compare(items.Item1, items.Item2);

            // Assert
            Assert.That(result, Is.Zero);
        }

        [Test]
        public void Compare_FirstItemAveragePointsHigher_ReturnsValueOne()
        {
            // Arrange
            var items = GetEqualItems();
            items.Item1.Details.AveragePoints = 1;
            items.Item2.Details.AveragePoints = 0;

            // Act
            var result = comparer.Compare(items.Item1, items.Item2);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Compare_FirstItemTotalPointsHigher_ReturnsValueOne()
        {
            // Arrange
            var items = GetEqualItems();
            items.Item1.Points = 1;
            items.Item2.Points = 0;

            // Act
            var result = comparer.Compare(items.Item1, items.Item2);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }


        private (RankingItem<CommunityDetails>, RankingItem<CommunityDetails>) GetEqualItems()
        {
            return (
                new RankingItem<CommunityDetails>() { Details = new CommunityDetails() { Name = string.Empty } },
                new RankingItem<CommunityDetails>() { Details = new CommunityDetails() { Name = string.Empty } });
        }
    }
}
