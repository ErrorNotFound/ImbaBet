using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Questions;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IDataStoreManager
    {
        public Task Initialize();

        public Task<Matchplan> GetMatchplanAsync();
        public Task ImportMatchplanAsync(Matchplan matchplan);
        public Task DeleteMatchplanAsync();
        
        public Task<IEnumerable<Bet>> GetBetsAsync();
        public Task<IEnumerable<Bet>> GetBetsOfPlayerAsync(Player player);
        public Task<IEnumerable<Bet>> GetBetsOfMatchAsync(Match match);
        public Task CreateBetsAsync(IEnumerable<Bet> bets);
        public Task UpdateBetsAsync(IEnumerable<Bet> bets);

        public Task<int> CreatePlayerAsync(Player player);
        public Task DeletePlayerAsync(int id);
        public Task<Player> GetPlayerByIdAsync(int id);
        public Task<IEnumerable<Player>> GetPlayersAsync();
        public Task UpdatePlayersAsync(IEnumerable<Player> players);

        public Task<IEnumerable<Community>> GetCommunitiesAsync();
        public Task<int> CreateCommunityAsync(Community community);
        public Task UpdateCommunitiesAsync(IEnumerable<Community> communities);
        public Task DeleteCommunityAsync(int communityId);

        public Task UpdateMatchesAsync(IEnumerable<Match> matches);

        public Task<IEnumerable<Setting>> GetSettingsAsync();
        public Task<T> GetCachedSettingValueAsync<T>(string id) where T : IConvertible;
        public Task<T> GetSettingValueAsync<T>(string id) where T : IConvertible;
        public Task ResetSettingAsync(string id);
        public Task SeedSettingsAsync();
        public Task SetSettingValueAsync<T>(string id, T value) where T : IConvertible;

        public Task CreatePlayerAnswersAsync(IEnumerable<PlayerAnswer> answers);
        public Task<IEnumerable<PlayerAnswer>> GetPlayerAnswersAsync();
        public Task<IEnumerable<PlayerAnswer>> GetPlayerAnswersOfPlayerAsync(Player player);
        public Task UpdatePlayerAnswersAsync(IEnumerable<PlayerAnswer> answers);
        public Task<IEnumerable<Question>> GetQuestionsAsync();

    }
}
