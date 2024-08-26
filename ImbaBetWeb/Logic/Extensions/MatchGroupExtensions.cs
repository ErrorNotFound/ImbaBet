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
            return mg.Matches.Select(a => a.TeamA).Union(mg.Matches.Select(b => b.TeamB)).ToList();
        }
    }
}
