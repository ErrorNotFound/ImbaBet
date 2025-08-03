using Allure.NUnit;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Logic.Ranking.Comparer;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.Test.Logic.Ranking.Comparer
{
    [AllureNUnit]
    public class GroupRankingComparerTests
    {
        private GroupRankingComparer comparer;

        [SetUp]
        public void Setup()
        {
            comparer = new GroupRankingComparer( new MatchGroup() { Name = string.Empty });
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
        public void Compare_FirstItemAverageStackRankHigher_ReturnsValueOne()
        {
            // Arrange
            var items = GetEqualItems();
            items.Item1.Details.Team.StackRank = 1;
            items.Item2.Details.Team.StackRank = 0;

            // Act
            var result = comparer.Compare(items.Item1, items.Item2);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Compare_FirstItemAveragePointsHigher_ReturnsValueOne()
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

        [Test]
        public void Compare_FirstItemGoalDifferenceHigher_ReturnsValueOne()
        {
            // Arrange
            var items = GetEqualItems();
            items.Item1.Details.Goals = 1;
            items.Item1.Details.GoalsAgainst = 0;
            items.Item2.Details.Goals = 0;
            items.Item2.Details.GoalsAgainst = 0;

            // Act
            var result = comparer.Compare(items.Item1, items.Item2);

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }


        private (RankingItem<TeamDetails>, RankingItem<TeamDetails>) GetEqualItems()
        {
            return (
                new RankingItem<TeamDetails>() { Details = new TeamDetails() { Team = new Team() { Name = string.Empty } } },
                new RankingItem<TeamDetails>() { Details = new TeamDetails() { Team = new Team() { Name = string.Empty } } });
        }
    }
}
