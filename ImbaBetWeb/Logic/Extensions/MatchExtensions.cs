using ImbaBetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImbaBetWeb.Logic.Extensions
{
    public static class MatchExtensions
    {
        public static bool HasTeamWon(this Match match, Team team)
        {
            if (!match.IsOver)
            {
                return false;
            }

            if (!match.HasTeam(team))
            {
                throw new Exception($"Team ({team.Name}) was not part of Match ({match.Id} {match.TeamA?.Name ?? match.AlternativeTeamAText} - {match.TeamB?.Name ?? match.AlternativeTeamBText})");
            }

            if (team == match.TeamA)
            {
                return match.GoalsA > match.GoalsB;
            }
            else
            {
                return match.GoalsB > match.GoalsA;
            }
        }

        public static bool HasTeamLost(this Match match, Team team)
        {
            if (!match.IsOver)
            {
                return false;
            }

            if (!match.HasTeam(team))
            {
                throw new Exception($"Team ({team.Name}) was not part of Match ({match.Id} {match.TeamA?.Name ?? match.AlternativeTeamAText} - {match.TeamB?.Name ?? match.AlternativeTeamBText})");
            }

            if (team == match.TeamA)
            {
                return match.GoalsA < match.GoalsB;
            }
            else
            {
                return match.GoalsB < match.GoalsA;
            }
        }

        public static bool HasTeamDrawed(this Match match, Team team)
        {
            if (!match.IsOver)
            {
                return false;
            }

            if (!match.HasTeam(team))
            {
                throw new Exception($"Team ({team.Name}) was not part of Match ({match.Id} {match.TeamA?.Name ?? match.AlternativeTeamAText} - {match.TeamB?.Name ?? match.AlternativeTeamBText})");
            }

            return match.GoalsA == match.GoalsB;
        }

        public static bool HasTeam(this Match match, Team team)
        {
            return team == match.TeamA || team == match.TeamB;
        }

        public static int GetGoals(this Match match, Team team)
        {
            if (!match.IsOver)
            {
                return 0;
            }

            if (!match.HasTeam(team))
            {
                throw new Exception($"Team ({team.Name}) was not part of Match ({match.Id} {match.TeamA?.Name ?? match.AlternativeTeamAText} - {match.TeamB?.Name ?? match.AlternativeTeamBText})");
            }

            return match.TeamA == team ? match.GoalsA : match.GoalsB;
        }

        public static int GetGoalsAgainst(this Match match, Team team)
        {
            if (!match.IsOver)
            {
                return 0;
            }

            if (!match.HasTeam(team))
            {
                throw new Exception($"Team ({team.Name}) was not part of Match ({match.Id} {match.TeamA?.Name ?? match.AlternativeTeamAText} - {match.TeamB?.Name ?? match.AlternativeTeamBText})");
            }

            return match.TeamA == team ? match.GoalsB : match.GoalsA;
        }

        public static bool CanBet(this Match match)
        {
            return !match.IsOver
                && match.TeamA != null
                && match.TeamB != null
                && DateTime.UtcNow < match.DateTime;
        }

        public static MatchResult GetMatchResult(this Match match)
        {
            if (!match.IsOver)
            {
                throw new Exception("Match is not over");
            }
            if (match.TeamA == null || match.TeamB == null)
            {
                throw new Exception("Teams not set");
            }

            var result = new MatchResult()
            {
                IsDraw = match.GoalsA == match.GoalsB,
                Winner = match.GoalsA == match.GoalsB ? null : match.GoalsA > match.GoalsB ? match.TeamA : match.TeamB
            };

            return result;
        }

    }
}
