using ImbaBetWeb.Data;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Logic.Ranking.Comparing;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Logic
{
    public class GameManager
    {
        private readonly ApplicationContext _context;
        private readonly SettingsManager _settingsManager;

        public GameManager(ApplicationContext context, SettingsManager settingsManager)
        {
            _context = context;
            _settingsManager = settingsManager;
        }

        public async Task<IList<Match>> GetMatchesAsync()
        {
            return await _context.Matches.ToListAsync();
        }

        public async Task<IList<MatchGroup>> GetMatchGroupsAsync()
        {
            return await _context.MatchGroups.ToListAsync();
        }

        public async Task<IList<Team>> GetTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task UpdateMatchesAsync(IList<Match> matches)
        {
            _context.Matches.UpdateRange(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<Match?> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            return match;
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


        private async Task<IList<RankingItem<TeamDetails>>> GetRankingInternalAsync(IList<Match> matches, IList<Team> teams, IComparer<RankingItem<TeamDetails>> comparer)
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

                item.Points =   item.Details.Wins * await _settingsManager.GetCachedSettingAsync<int>(SettingNames.MATCH_POINTS_PER_WIN) 
                                + item.Details.Draws * await _settingsManager.GetCachedSettingAsync<int>(SettingNames.MATCH_POINTS_PER_DRAW);

                list.Add(item);
            }

            RankingHelper.SortAndSetRanks(list, comparer);

            return list;
        }
    }
}
