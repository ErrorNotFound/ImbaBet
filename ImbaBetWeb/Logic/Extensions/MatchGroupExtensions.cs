using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Extensions
{
    public static class MatchGroupExtensions
    {
        public static List<Team> GetTeamList(this MatchGroup mg)
        {
            var teamsA = mg.Matches.Where(m => m.TeamA != null).Select(m => m.TeamA!);
            var teamsB = mg.Matches.Where(m => m.TeamB != null).Select(m => m.TeamB!);

            return teamsA.Union(teamsB).ToList();
        }
    }
}
