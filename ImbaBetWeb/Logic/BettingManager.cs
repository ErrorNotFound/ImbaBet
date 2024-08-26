using ImbaBetWeb.Data;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Logic.Ranking.Comparing;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Logic
{
    public class BettingManager
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SettingsManager _settingsManager;

        public BettingManager(
            ApplicationContext context, 
            UserManager<ApplicationUser> userManager,
            SettingsManager settingsManager)
        {
            _context = context;
            _userManager = userManager;
            _settingsManager = settingsManager;
        }

        public async Task<IList<Bet>> GetOpenBetsForUserAsync(ApplicationUser user)
        {
            var betableMatches = await GetMatchesInternalAsync((match) => { return match.CanBet(); });
            var activeBetsByUser = await GetBetsInternalAsync((bet) => { return bet.User == user && bet.Match.CanBet(); });

            var missingBetObjects = betableMatches.Where(match => !activeBetsByUser.Any(bet => bet.Match == match)).Select(bet => new Bet()
            {
                Match = bet,
                MatchId = bet.Id,
                User = user,
                UserId = user.Id
            }).ToList();

            return activeBetsByUser.Concat(missingBetObjects).OrderBy(x => x.Match.DateTime).ToList();
        }

        public async Task<IList<Bet>> GetActiveBetsForUserAsync(ApplicationUser user)
        {
            Func<Bet, bool> predicate = (bet) => { return bet.User == user && !bet.Match.IsOver && DateTime.UtcNow >= bet.Match.DateTime; };
            return await GetBetsInternalAsync(predicate);
        }

        public async Task<IList<Bet>> GetClosedBetsForUserAsync(ApplicationUser user)
        {
            Func<Bet, bool> predicate = (bet) => { return bet.User == user && bet.Match.IsOver; };
            return await GetBetsInternalAsync(predicate);
        }

        public async Task<IList<Bet>> GetActiveBetsForMatchAsync(int matchId)
        {
            Func<Bet, bool> predicate = (bet) => { return bet.MatchId == matchId && !bet.Match.IsOver && DateTime.UtcNow >= bet.Match.DateTime; };
            return await GetBetsInternalAsync(predicate);
        }

        public async Task<IList<Bet>> GetClosedBetsForMatchAsync(int matchId)
        {
            Func<Bet, bool> predicate = (bet) => { return bet.MatchId == matchId && bet.Match.IsOver; };
            return await GetBetsInternalAsync(predicate);
        }

        public async Task<bool> UpdateBetsAsync(IList<Bet> bets)
        {
            if(bets == null)
            {  
                return false; 
            }

            var allMatches = await GetMatchesInternalAsync((match) => true);
            var allowedBets = bets.Where((bet) => { return allMatches.SingleOrDefault(match => match.Id == bet.MatchId)?.CanBet() ?? false; });

            _context.Bets.UpdateRange(allowedBets);
            await _context.SaveChangesAsync();

            return bets.Count == allowedBets.Count();
        }

        public async Task UpdatePointsAsync()
        {    
            // Update Bets
            var allBets = await GetBetsInternalAsync((bet) => true);
            foreach (var bet in allBets)
            {
                bet.Points = await GetPointsForBet(bet);
            }
            _context.Bets.UpdateRange(allBets);
            await _context.SaveChangesAsync();

            // Update Users
            var allUsers = await GetUsersInternalAsync((user) => true);
            foreach (var user in allUsers)
            {
                user.Points = user.Bets.Sum(b => b.Points);
            }
            _context.Users.UpdateRange(allUsers);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<RankingItem<UserDetails>>> GetUserRankingAsync()
        {
            var users = await GetUsersInternalAsync((user) => true);
            var list = GetRankingOfUsersInternal(users);

            return list;
        }

        public async Task<IList<RankingItem<UserDetails>>> GetUserRankingOfCommunityAsync(int communityId)
        {
            var users = await GetUsersInternalAsync((user) => { return user.MemberOfCommunityId == communityId; });
            var list = GetRankingOfUsersInternal(users);

            return list;
        }

        public async Task<IList<RankingItem<CommunityDetails>>> GetCommunityRankingAsync()
        {
            var communities = await _context.Communities.Include(c => c.Members).ToListAsync();
            var minMemberCount = await _settingsManager.GetSettingAsync<int>(SettingNames.MIN_MEMBER_COUNT_FOR_RANKING);

            var list = communities.Where(x => x.Members.Count >= minMemberCount).Select(community =>
            {
                var totalPoints = community.Members.Sum(member => member.Points);
                var memberCount = community.Members.Count;

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

            RankingHelper.SortAndSetRanks(list, new CommunityComparer());

            return list;
        }

        private async Task<IList<Bet>> GetBetsInternalAsync(Func<Bet, bool> predicate)
        {
            var allBets = await _context.Bets
                .Include(b => b.User)
                .Include(b => b.Match)
                .ThenInclude(m => m.TeamA)
                .Include(b => b.Match)
                .ThenInclude(m => m.TeamB).ToListAsync();

            var matchedBets = allBets.Where(predicate).OrderBy(b => b.Match.DateTime).ToList();

            return matchedBets;
        }

        private async Task<IList<Match>> GetMatchesInternalAsync(Func<Match, bool> predicate)
        {
            var allMatches = await _context.Matches.ToListAsync();
            var matchedMatches = allMatches.Where(predicate).OrderBy(m => m.DateTime).ToList();

            return matchedMatches;
        }

        private async Task<IList<ApplicationUser>> GetUsersInternalAsync(Func<ApplicationUser, bool> predicate)
        {
            var allUsers = await _context.Users.ToListAsync();
            var matchedUsers = allUsers.Where(predicate).OrderBy(u => u.UserName).ToList();

            return matchedUsers;
        }

        private IList<RankingItem<UserDetails>> GetRankingOfUsersInternal(IList<ApplicationUser> users)
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

            RankingHelper.SortAndSetRanks(rankingList, new UserComparer());
            return rankingList;
        }


        private async Task<int> GetPointsForBet(Bet bet)
        {
            var match = bet.Match;
            if (!match.IsOver)
                return 0;

            // Exact bet
            if (match.GoalsA == bet.GoalsA && match.GoalsB == bet.GoalsB)
            {
                return await _settingsManager.GetCachedSettingAsync<int>(SettingNames.BETTING_POINTS_EXACT_RESULT);
            }

            var result = match.GetMatchResult();
            var correctTendency = result.Winner == bet.GetSuggestedWinner();
            var correctGoalDiff = match.GoalsA - match.GoalsB == bet.GoalsA - bet.GoalsB;

            if (correctTendency)
            {
                return correctGoalDiff ? await _settingsManager.GetCachedSettingAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY_AND_DIFFERENCE) : await _settingsManager.GetCachedSettingAsync<int>(SettingNames.BETTING_POINTS_CORRECT_TENDENCY);
            }

            return 0;
        }
    }
}
