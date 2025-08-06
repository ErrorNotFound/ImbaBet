using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IMatchGroupStore
    {
        Task<int> CreateAsync(NMatchGroup obj);
        Task<NMatchGroup> GetAsync(int id);
        Task<IEnumerable<NMatchGroup>> GetAllAsync();
        Task UpdateAsync(NMatchGroup obj);
        Task DeleteAsync(NMatchGroup key);

        Task EnsureInitializedAsync();
    }
}
