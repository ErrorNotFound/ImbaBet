using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IMatchGroupStore
    {
        Task<int> CreateAsync(MatchGroup obj);
        Task<MatchGroup> GetAsync(int id);
        Task<IEnumerable<MatchGroup>> GetAllAsync();
        Task UpdateAsync(MatchGroup obj);
        Task DeleteAsync(MatchGroup key);

        Task EnsureInitializedAsync();
    }
}
