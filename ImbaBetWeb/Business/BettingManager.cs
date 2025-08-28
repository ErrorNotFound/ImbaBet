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
        /// Returns a list of bets that the user has not betted on yet
        /// </summary>
        public async Task<IEnumerable<Bet>> GetOpenBetsOfUserAsync(BettingUser user)
        {
            var matchplan = await _dataStoreManager.GetMatchplanAsync();
            var betsOfUser = await _dataStoreManager.GetBetsOfUserAsync(user);

            var betableMatches = matchplan.Matches.Where(match => match.CanBet());

            var matchesNotBetOnByUser = betableMatches.Where(m => !betsOfUser.Any(b => b.MatchId == m.Id));

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
        public async Task<IEnumerable<Bet>> GetActiveBetsOfUserAsync(BettingUser user)
        {
            var betsOfUser = await _dataStoreManager.GetBetsOfUserAsync(user);
            return betsOfUser.Where(bet => bet.IsActiveBet());
        }

        /// <summary>
        /// Returns a list of bets that the user has betted on and can't be modified anymore
        /// </summary>
        public async Task<IEnumerable<Bet>> GetClosedBetsOfUserAsync(BettingUser user)
        {
            var betsOfUser = await _dataStoreManager.GetBetsOfUserAsync(user);
            return betsOfUser.Where(bet => bet.IsClosedBet());
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
