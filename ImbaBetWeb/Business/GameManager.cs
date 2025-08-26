using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Business.Ranking;
using ImbaBetWeb.Business.Ranking.Comparer;
using ImbaBetWeb.Business.Ranking.Details;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class GameManager(IDataStoreManager dataStore)
    {
        private readonly IDataStoreManager _dataStoreManager = dataStore;

        public async Task<IEnumerable<Match>> GetMatchesAsync()
        {
            return await _dataStoreManager.GetMatchesAsync();
        }

        public async Task<IEnumerable<MatchGroup>> GetMatchGroupsAsync()
        {
            return await _dataStoreManager.GetMatchGroupsAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return await _dataStoreManager.GetTeamsAsync();
        }

        public async Task UpdateMatchesAsync(IEnumerable<Match> matches)
        {
            await _dataStoreManager.UpdateMatchesAsync(matches);
        }

        public async Task<Match?> GetMatchByIdAsync(int matchId)
        {
            var matches = await _dataStoreManager.GetMatchesAsync(); 
            return matches.SingleOrDefault(m => m.Id == matchId);
        }


        public async Task<IList<RankingItem<TeamDetails>>> GetTeamRankingAsync()
        {
            var matches = await GetMatchesAsync();
            var teams = await GetTeamsAsync();

            var ranking = await GetRankingInternalAsync(matches, teams, new TeamRankingComparer());

            return ranking;
        }

        public async Task<Dictionary<MatchGroup, IList<RankingItem<TeamDetails>>>> GetGroupRankingAsync()
        {
            var matchGroups = await GetMatchGroupsAsync();
            var matchGroupsWithGroupRanking = matchGroups.Where(mg => mg.HasGroupRanking);

            var result = new Dictionary<MatchGroup, IList<RankingItem<TeamDetails>>>();
            foreach (var matchGroup in matchGroupsWithGroupRanking)
            {
   

                var rankingForGroup = await GetRankingInternalAsync(matchGroup.Matches, matchGroup.GetTeamList(), new GroupRankingComparer(matchGroup));
                result.Add(matchGroup, rankingForGroup);
            }

            return result;
        }


        private async Task<IList<RankingItem<TeamDetails>>> GetRankingInternalAsync(IEnumerable<Match> matches, IEnumerable<Team> teams, IComparer<RankingItem<TeamDetails>> comparer)
        {
            var list = new List<RankingItem<TeamDetails>>();

            foreach (var team in teams)
            {
                var teamMatches = matches.Where(m => m.HasTeam(team));
                var item = new RankingItem<TeamDetails>()
                {
                    Details = new TeamDetails()
                    {
                        Team = team,
                        MatchesPlayed = teamMatches.Count(m => m.IsOver),
                        Wins = teamMatches.Count(m => m.HasTeamWon(team)),
                        Draws = teamMatches.Count(m => m.HasTeamDrawed(team)),
                        Lost = teamMatches.Count(m => m.HasTeamLost(team)),
                        Goals = teamMatches.Sum(m => m.GetGoals(team)),
                        GoalsAgainst = teamMatches.Sum(m => m.GetGoalsAgainst(team))
                    }
                };

                item.Points =   item.Details.Wins * await _dataStoreManager.GetCachedSettingValueAsync<int>(SettingNames.MATCH_POINTS_PER_WIN) 
                                + item.Details.Draws * await _dataStoreManager.GetCachedSettingValueAsync<int>(SettingNames.MATCH_POINTS_PER_DRAW);

                list.Add(item);
            }

            RankingHelper.SortDescendingAndSetRanks(list, comparer);

            return list;
        }
    }
}
