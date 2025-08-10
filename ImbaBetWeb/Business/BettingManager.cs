using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Business.Ranking;
using ImbaBetWeb.Business.Ranking.Comparer;
using ImbaBetWeb.Business.Ranking.Details;
using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class BettingManager
    {
        private readonly SettingsManager _settingsManager;
        private readonly IDataStoreManager _dataStoreManager;

        public BettingManager(
            IDataStoreManager dataStoreManager, 
            SettingsManager settingsManager)
        {
            _dataStoreManager = dataStoreManager;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Returns a list of bets that the user has not betted on yet
        /// </summary>
        public async Task<IEnumerable<Bet>> GetOpenBetsForUserAsync(BettingUser user)
        {
            var gameplan = await _dataStoreManager.GetGameplanAsync();
            var bets = await _dataStoreManager.GetBetsAsync();

            var betableMatches = gameplan.Matches.Where(match => match.CanBet());
            var betsByUser = bets.Where(bet => bet.UserId == user.Id);

            var matchesNotBetOnByUser = betableMatches.Where(m => !betsByUser.Any(b => b.MatchId == m.Id));

            var openBets = matchesNotBetOnByUser.Select(m => new Bet()
            {
                Match = m,
                MatchId = m.Id,
                UserId = user.Id,
                User = user
            });

            return openBets.ToList();
        }

        /// <summary>
        /// Returns a list of bets that the user has betted on but can still be modified
        /// </summary>
        public async Task<IEnumerable<Bet>> GetActiveBetsForUserAsync(BettingUser user)
        {
            var bets = await _dataStoreManager.GetBetsAsync();
            return bets.Where(bet => bet.User == user && bet.IsActiveBet());
        }

        /// <summary>
        /// Returns a list of bets that the user has betted on and can't be modified anymore
        /// </summary>
        public async Task<IEnumerable<Bet>> GetClosedBetsForUserAsync(BettingUser user)
        {
            var bets = await _dataStoreManager.GetBetsAsync();
            return bets.Where(bet => bet.User == user && bet.IsClosedBet());
        }

        public async Task<IEnumerable<Bet>> GetActiveBetsForMatchAsync(int matchId)
        {
            var bets = await _dataStoreManager.GetBetsAsync();
            return bets.Where(bet => bet.MatchId == matchId && bet.IsActiveBet());
        }

        public async Task<IEnumerable<Bet>> GetClosedBetsForMatchAsync(int matchId)
        {
            var bets = await _dataStoreManager.GetBetsAsync();
            return bets.Where(bet => bet.MatchId == matchId && bet.IsClosedBet());
        }

        public async Task<bool> UpdateBetsAsync(IEnumerable<Bet> bets)
        {
            if(bets == null)
            {  
                return false; 
            }

            var gameplan = await _dataStoreManager.GetGameplanAsync();
            var allowedBets = bets.Where((bet) => { return gameplan.Matches.SingleOrDefault(match => match.Id == bet.MatchId)?.CanBet() ?? false; });

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

            // Update Users
            var users = await _dataStoreManager.GetUsersAsync();
            foreach (var user in users)
            {
                var userBets = allBets.Where(bet => bet.UserId == user.Id);
                user.Points = userBets.Sum(b => b.Points);
            }
            await _dataStoreManager.UpdateUsersAsync(users);
        }

        public async Task<IList<RankingItem<UserDetails>>> GetUserRankingAsync()
        {
            var users = await _dataStoreManager.GetUsersAsync();
            var list = GetRankingOfUsersInternal(users);

            return list;
        }

        public async Task<IList<RankingItem<UserDetails>>> GetUserRankingOfCommunityAsync(int communityId)
        {
            var users = await _dataStoreManager.GetUsersAsync();
            var communityUsers = users.Where(user => user.MemberOfCommunityId == communityId);
            var list = GetRankingOfUsersInternal(communityUsers);

            return list;
        }

        public async Task<IList<RankingItem<CommunityDetails>>> GetCommunityRankingAsync()
        {
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var minMemberCount = await _settingsManager.GetSettingValueAsync<int>(SettingNames.MIN_MEMBER_COUNT_FOR_RANKING);

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

        private IList<RankingItem<UserDetails>> GetRankingOfUsersInternal(IEnumerable<BettingUser> users)
        {
            var rankingList = new List<RankingItem<UserDetails>>();

            foreach (var user in users)
            {
                rankingList.Add(new RankingItem<UserDetails>()
                {
                    Details = new UserDetails() { User = user },
                    Points = user.Points
                });
            }

            RankingHelper.SortDescendingAndSetRanks(rankingList, new UserComparer());
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
                return await _settingsManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_EXACT_RESULT);
            }

            var result = match.GetMatchResult();
            var correctTendency = result.Winner == bet.GetSuggestedWinner();
            var correctGoalDiff = match.GoalsA - match.GoalsB == bet.GoalsA - bet.GoalsB;

            if (correctTendency)
            {
                return correctGoalDiff ? await _settingsManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY_AND_DIFFERENCE) : await _settingsManager.GetCachedSettingValueAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY);
            }

            return 0;
        }
    }
}
