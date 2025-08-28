using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IDataStoreManager
    {
        public Task Initialize();

        public Task<Matchplan> GetMatchplanAsync();
        public Task ImportMatchplanAsync(Matchplan matchplan);
        public Task DeleteMatchplanAsync();
        
        public Task<IEnumerable<Bet>> GetBetsAsync();
        public Task<IEnumerable<Bet>> GetBetsOfUserAsync(BettingUser user);
        public Task<IEnumerable<Bet>> GetBetsOfMatchAsync(Match match);
        public Task UpdateBetsAsync(IEnumerable<Bet> bets);

        public Task<int> CreateUserAsync(BettingUser user);
        public Task<BettingUser> GetUserByIdAsync(int id);
        public Task<IEnumerable<BettingUser>> GetUsersAsync();
        public Task UpdateUsersAsync(IEnumerable<BettingUser> users);

        public Task<IEnumerable<Community>> GetCommunitiesAsync();
        public Task AddCommunityAsync(Community community);
        public Task UpdateCommunitiesAsync(IEnumerable<Community> communities);
        public Task DeleteCommunityAsync(int communityId);

        public Task UpdateMatchesAsync(IEnumerable<Match> matches);

        public Task<IEnumerable<Setting>> GetSettingsAsync();
        public Task<T> GetCachedSettingValueAsync<T>(string id) where T : IConvertible;
        public Task<T> GetSettingValueAsync<T>(string id) where T : IConvertible;
        public Task ResetSettingAsync(string id);
        public Task SeedSettingsAsync();
        public Task SetSettingValueAsync<T>(string id, T value) where T : IConvertible;
    }
}
