using Allure.NUnit;
using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.Tests.Unit.Business.Extensions
{
    [AllureNUnit]
    public class BetExtensionsTests
    {
        [Test]
        public void IsActiveBet_MatchNotOverAndInFuture_ReturnsTrue()
        {
            // Arrange
            var bet = new Bet()
            {
                Match = new Match()
                {
                    IsOver = false,
                    DateTime = DateTime.UtcNow.AddHours(1)
                }
            };

            // Act
            var result = bet.IsActiveBet();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsActiveBet_MatchIsOver_ReturnsFalse()
        {
            // Arrange
            var bet = new Bet()
            {
                Match = new Match()
                {
                    IsOver = true
                }
            };

            // Act
            var result = bet.IsActiveBet();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsActiveBet_MatchIsInPast_ReturnsFalse()
        {
            // Arrange
            var bet = new Bet()
            {
                Match = new Match()
                {
                    IsOver = false,
                    DateTime = DateTime.UtcNow.AddHours(-1)
                }
            };

            // Act
            var result = bet.IsActiveBet();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsClosedBet_MatchIsOver_ReturnsTrue()
        {
            // Arrange
            var bet = new Bet()
            {
                Match = new Match()
                {
                    IsOver = true
                }
            };

            // Act
            var result = bet.IsClosedBet();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsClosedBet_MatchIsNotOver_ReturnsTrue()
        {
            // Arrange
            var bet = new Bet()
            {
                Match = new Match()
                {
                    IsOver = false
                }
            };

            // Act
            var result = bet.IsClosedBet();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetSuggestedWinner_PlayerBetDraw_ReturnsNull()
        {
            // Arrange
            var bet = new Bet()
            {
                GoalsA = 2,
                GoalsB = 2,
            };

            // Act
            var result = bet.GetSuggestedWinner();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetSuggestedWinner_TeamAHasMoreGoals_ReturnsTeamA()
        {
            // Arrange
            var teamA = new Team() { Name = "Team A" };
            var teamB = new Team() { Name = "Team B" };

            var bet = new Bet()
            {
                GoalsA = 2,
                GoalsB = 0,
                Match = new Match()
                {
                    TeamA = teamA,
                    TeamB = teamB
                }
            };

            // Act
            var result = bet.GetSuggestedWinner();

            // Assert
            Assert.That(result, Is.EqualTo(teamA));
        }

        [Test]
        public void GetSuggestedWinner_TeamBHasMoreGoals_ReturnsTeamB()
        {
            // Arrange
            var teamA = new Team() { Name = "Team A" };
            var teamB = new Team() { Name = "Team B" };

            var bet = new Bet()
            {
                GoalsA = 0,
                GoalsB = 2,
                Match = new Match()
                {
                    TeamA = teamA,
                    TeamB = teamB
                }
            };

            // Act
            var result = bet.GetSuggestedWinner();

            // Assert
            Assert.That(result, Is.EqualTo(teamB));
        }
    }
}
