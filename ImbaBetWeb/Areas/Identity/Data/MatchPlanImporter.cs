using ImbaBetWeb.Data;
using ImbaBetWeb.Models;
using System.Globalization;
using System.Xml.Linq;

namespace ImbaBetWeb.Areas.Identity.Data
{
    public static class MatchPlanImporter
    {
        private const string filename = "MatchPlanEM2024.xml";

        public async static Task ImportAsync(ApplicationContext databaseContext)
        {
            var xdoc = XDocument.Load(filename);

            var teams1 = from t in xdoc.Descendants("Team")
                         select new Team()
                         {
                             Name = t.Attribute("Name").Value,
                             FlagCountryCode = t.Attribute("Icon")?.Value
                         };

            var teams = teams1.ToList();

            var test = teams.FirstOrDefault(x => x.Name == "Deutschland");

            var matchGroups = new List<MatchGroup>();
            var allMatches = new List<Match>();

            int order = 1;
            foreach (var mg in xdoc.Descendants("MatchGroup"))
            {
                var name = mg.Attribute("Name")?.Value ?? "Unknown MatchGroup Name";
                var groupRanking = bool.Parse(mg.Attribute("HasGroupRanking")?.Value ?? "false");

                var matchGroup = new MatchGroup()
                {
                    Name = name,
                    HasGroupRanking = groupRanking,
                    StackRank = order++
                };

                var matches = new List<Match>();
                foreach (var m in mg.Descendants("Match"))
                {
                    var match = new Match();
                    match.MatchGroup = matchGroup;
                    match.TeamA = m.Attribute("TeamA") != null ? teams.Single(x => x.Name == m.Attribute("TeamA").Value) : null;
                    match.TeamB = m.Attribute("TeamB") != null ? teams.Single(x => x.Name == m.Attribute("TeamB").Value) : null;
                    match.DateTime = DateTime.Parse(m.Attribute("DateTime").Value, CultureInfo.InvariantCulture);

                    match.AlternativeTeamAText = m.Attribute("AlternativeTeamAText") != null ? m.Attribute("AlternativeTeamAText").Value : null;
                    match.AlternativeTeamBText = m.Attribute("AlternativeTeamBText") != null ? m.Attribute("AlternativeTeamBText").Value : null;

                    matches.Add(match);
                    allMatches.Add(match);
                }
                matchGroup.Matches = matches;

                matchGroups.Add(matchGroup);
            }

            databaseContext.Teams.AddRange(teams);
            databaseContext.Matches.AddRange(allMatches);
            databaseContext.MatchGroups.AddRange(matchGroups);
            await databaseContext.SaveChangesAsync();
        }
    }
}
