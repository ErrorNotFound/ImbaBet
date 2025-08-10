using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess
{
    public class DataStoreManager : IDataStoreManager
    {
        private IBetStore betStore;
        private IBettingUserStore bettingUserStore;
        private ICommunityStore communityStore;
        private IMatchGroupStore matchGroupStore;
        private IMatchStore matchStore;
        private ISettingStore settingStore;
        private ITeamStore teamStore;

        public DataStoreManager(string connectionString)
        {
            betStore = new BetStore(connectionString);
            bettingUserStore = new BettingUserStore(connectionString);
            communityStore = new CommunityStore(connectionString);
            matchGroupStore = new MatchGroupStore(connectionString);
            matchStore = new MatchStore(connectionString);
            settingStore = new SettingStore(connectionString);
            teamStore = new TeamStore(connectionString);
        }
        public async Task Initialize()
        {
            await Task.WhenAll(
                betStore.EnsureInitializedAsync(),
                bettingUserStore.EnsureInitializedAsync(),
                communityStore.EnsureInitializedAsync(),
                matchGroupStore.EnsureInitializedAsync(),
                matchStore.EnsureInitializedAsync(),
                settingStore.EnsureInitializedAsync(),
                teamStore.EnsureInitializedAsync());
        }

        public async Task<(IEnumerable<MatchGroup> MatchGroups, IEnumerable<Match> Matches, IEnumerable<Team> Teams)> GetGameplanAsync()
        {
            var matchGroups = await matchGroupStore.GetAllAsync();
            var matches = await matchStore.GetAllAsync();
            var teams = await teamStore.GetAllAsync();

            foreach(var match in matches)
            {
                match.TeamA = teams.SingleOrDefault(x => match.TeamATeamId == x.Id);
                match.TeamB = teams.SingleOrDefault(x => match.TeamBTeamId == x.Id);
                match.MatchGroup = matchGroups.Single(x => match.MatchGroupId == x.Id);
            }

            foreach(var matchGroup in matchGroups)
            {
                matchGroup.Matches = matches.Where(m => matchGroup.Id == m.MatchGroupId).ToList();
            }

            return (matchGroups, matches, teams);
        }

        public async Task<IEnumerable<Bet>> GetBetsAsync()
        {
            var bets = await betStore.GetAllAsync();
            var matches = await matchStore.GetAllAsync();
            var users = await bettingUserStore.GetAllAsync();

            foreach (var bet in bets)
            {
                bet.Match = matches.Single(m => bet.MatchId == m.Id);
                bet.User = users.Single(u => bet.UserId == u.Id);
            }

            return bets;
        }

        public async Task UpdateBetsAsync(IEnumerable<Bet> bets)
        {
            foreach(var bet in bets)
            {
                await betStore.UpdateAsync(bet);
            }
        }

        public async Task<IEnumerable<BettingUser>> GetUsersAsync()
        {
            return await bettingUserStore.GetAllAsync();
        }

        public async Task UpdateUsersAsync(IEnumerable<BettingUser> users)
        {
            foreach (var user in users)
            {
                await bettingUserStore.UpdateAsync(user);
            }
        }

        public async Task<IEnumerable<Community>> GetCommunitiesAsync()
        {
            var communities = await communityStore.GetAllAsync();
            var users = await bettingUserStore.GetAllAsync();

            foreach (var community in communities)
            {
                community.Members = users.Where(u => community.Id == u.MemberOfCommunityId).ToList();
                community.Owner = users.Single(u => community.OwnerId == u.Id);
            }

            return communities;
        }
    }
}
