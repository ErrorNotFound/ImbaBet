using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IDataStoreManager
    {
        public Task Initialize();

        public Task<(IEnumerable<MatchGroup> MatchGroups, IEnumerable<Match> Matches, IEnumerable<Team> Teams)> GetGameplanAsync();
        public Task<IEnumerable<Bet>> GetBetsAsync();
        public Task UpdateBetsAsync(IEnumerable<Bet> bets);

        public Task<int> CreateUserAsync(BettingUser user);
        public Task<BettingUser> GetUserByIdAsync(int id);
        public Task<IEnumerable<BettingUser>> GetUsersAsync();
        public Task UpdateUsersAsync(IEnumerable<BettingUser> users);

        public Task<IEnumerable<Community>> GetCommunitiesAsync();
        public Task AddCommunityAsync(Community community);
        public Task UpdateCommunitiesAsync(IEnumerable<Community> communities);
        public Task DeleteCommunityAsync(int communityId);

        public Task<IEnumerable<Match>> GetMatchesAsync();
        public Task UpdateMatchesAsync(IEnumerable<Match> matches);
        public Task<IEnumerable<MatchGroup>> GetMatchGroupsAsync();
        public Task<IEnumerable<Team>> GetTeamsAsync();

        public Task<IEnumerable<Setting>> GetSettingsAsync();
        public Task<T> GetCachedSettingValueAsync<T>(string key) where T : IConvertible;
        public Task<T> GetSettingValueAsync<T>(string key) where T : IConvertible;
        public Task ResetSettingAsync(string key);
        public Task SeedSettingsAsync();
        public Task SetSettingValueAsync<T>(string key, T value) where T : IConvertible;
    }
}
