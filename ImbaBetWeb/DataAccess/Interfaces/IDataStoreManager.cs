using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IDataStoreManager
    {
        public Task Initialize();

        public Task<(IEnumerable<MatchGroup> MatchGroups, IEnumerable<Match> Matches, IEnumerable<Team> Teams)> GetGameplanAsync();
        public Task<IEnumerable<Bet>> GetBetsAsync();
        public Task UpdateBetsAsync(IEnumerable<Bet> bets);

        public Task<IEnumerable<BettingUser>> GetUsersAsync();
        public Task UpdateUsersAsync(IEnumerable<BettingUser> users);

        public Task<IEnumerable<Community>> GetCommunitiesAsync();


    }
}
