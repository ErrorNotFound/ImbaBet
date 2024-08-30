using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Logic.Ranking.Comparing;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
