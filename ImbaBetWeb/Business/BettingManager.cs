using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Business.Ranking;
using ImbaBetWeb.Business.Ranking.Comparer;
using ImbaBetWeb.Business.Ranking.Details;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class BettingManager(
        IDataStoreManager dataStoreManager)
    {
        private readonly IDataStoreManager _dataStoreManager = dataStoreManager;

        /// <summary>
        /// Returns a list of bets that the player has not betted on yet
        /// </summary>
        public async Task<IEnumerable<Bet>> GetOpenBetsOfPlayerAsync(Player player)
        {
            var matchplan = await _dataStoreManager.GetMatchplanAsync();
            var betsOfPlayer = await _dataStoreManager.GetBetsOfPlayerAsync(player);

            var betableMatches = matchplan.Matches.Where(match => match.CanBet());

            var matchesNotBetOnByPlayer = betableMatches.Where(m => !betsOfPlayer.Any(b => b.MatchId == m.Id));

            var openBets = matchesNotBetOnByPlayer.Select(m => new Bet()
            {
                Match = m,
                MatchId = m.Id,
                PlayerId = player.Id,
                Player = player
            });

            return openBets.ToList();
        }

        /// <summary>
        /// Returns a list of bets that the player has betted on but can still be modified
        /// </summary>
        public async Task<IEnumerable<Bet>> GetActiveBetsOfPlayerAsync(Player player)
        {
            var betsOfPlayer = await _dataStoreManager.GetBetsOfPlayerAsync(player);
            return betsOfPlayer.Where(bet => bet.IsActiveBet());
        }

        /// <summary>
        /// Returns a list of bets that the player has betted on and can't be modified anymore
        /// </summary>
        public async Task<IEnumerable<Bet>> GetClosedBetsOfPlayerAsync(Player user)
        {
            var betsOfPlayer = await _dataStoreManager.GetBetsOfPlayerAsync(user);
            return betsOfPlayer.Where(bet => bet.IsClosedBet());
        }

        public async Task<IEnumerable<Bet>> GetActiveBetsOfMatchAsync(Match match)
        {
            var betsOfMatch = await _dataStoreManager.GetBetsOfMatchAsync(match);
            return betsOfMatch.Where(bet => bet.IsActiveBet());
        }

        public async Task<IEnumerable<Bet>> GetClosedBetsOfMatchAsync(Match match)
        {
            var betsOfMatch = await _dataStoreManager.GetBetsOfMatchAsync(match);
            return betsOfMatch.Where(bet => bet.IsClosedBet());
        }

        public async Task<bool> UpdateBetsAsync(IEnumerable<Bet> bets)
        {
            if(bets == null)
            {  
                return false; 
            }

            var matchplan = await _dataStoreManager.GetMatchplanAsync();
            var allowedBets = bets.Where((bet) => { return matchplan.Matches.SingleOrDefault(match => match.Id == bet.MatchId)?.CanBet() ?? false; });

            await _dataStoreManager.UpdateBetsAsync(allowedBets);

            return bets.Count() == allowedBets.Count();
        }

        public async Task UpdatePointsAsync()
        {    
            // Update Bets
            var allBets = await _dataStoreManager.GetBetsAsync();
            foreach (var bet in allBets)
            {
                bet.Points = await GetPointsForBet(bet);
            }
            await _dataStoreManager.UpdateBetsAsync(allBets);

            // Update Players
            var players = await _dataStoreManager.GetPlayersAsync();
            foreach (var player in players)
            {
                var playerBets = allBets.Where(bet => bet.PlayerId == player.Id);
                player.Points = playerBets.Sum(b => b.Points);
            }
            await _dataStoreManager.UpdatePlayersAsync(players);
        }

        public async Task<IList<RankingItem<PlayerDetails>>> GetPlayerRankingAsync()
        {
            var players = await _dataStoreManager.GetPlayersAsync();
            var list = GetRankingOfPlayersInternal(players);

            return list;
        }

        public async Task<IList<RankingItem<PlayerDetails>>> GetPlayerRankingOfCommunityAsync(int communityId)
        {
            var players = await _dataStoreManager.GetPlayersAsync();
            var communityPlayers = players.Where(user => user.MemberOfCommunityId == communityId);
            var list = GetRankingOfPlayersInternal(communityPlayers);

            return list;
        }

        public async Task<IList<RankingItem<CommunityDetails>>> GetCommunityRankingAsync()
        {
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var minMemberCount = await _dataStoreManager.GetSettingValueAsync<int>(SettingNames.MIN_MEMBER_COUNT_FOR_RANKING);

            var list = communities.Where(x => x.Members.Count() >= minMemberCount).Select(community =>
            {
                var totalPoints = community.Members.Sum(member => member.Points);
                var memberCount = community.Members.Count();

                var item = new RankingItem<CommunityDetails>
                {
                    Details = new CommunityDetails()
                    {
                        Name = community.Name,
                        MemberCount = memberCount,
                        AveragePoints = decimal.Divide(totalPoints, memberCount)
                    },
                    Points = totalPoints
                };

                return item;
            }).ToList();

            RankingHelper.SortDescendingAndSetRanks(list, new CommunityComparer());

            return list;
        }

        private IList<RankingItem<PlayerDetails>> GetRankingOfPlayersInternal(IEnumerable<Player> players)
        {
            var rankingList = new List<RankingItem<PlayerDetails>>();

            foreach (var player in players)
            {
                rankingList.Add(new RankingItem<PlayerDetails>()
                {
                    Details = new PlayerDetails() { Player = player },
                    Points = player.Points
                });
            }

            RankingHelper.SortDescendingAndSetRanks(rankingList, new PlayerComparer());
            return rankingList;
        }


        private async Task<int> GetPointsForBet(Bet bet)
        {
            var match = bet.Match!;
            if (!match.IsOver)
                return 0;

            // Exact bet
            if (match.GoalsA == bet.GoalsA && match.GoalsB == bet.GoalsB)
            {
                return await _dataStoreManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_EXACT_RESULT);
            }

            var result = match.GetMatchResult();
            var correctTendency = result.Winner == bet.GetSuggestedWinner();
            var correctGoalDiff = match.GoalsA - match.GoalsB == bet.GoalsA - bet.GoalsB;

            if (correctTendency)
            {
                return correctGoalDiff ? await _dataStoreManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY_AND_DIFFERENCE) : await _dataStoreManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY);
            }

            return 0;
        }
    }
}
