using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ICommunityStore
    {
        Task<int> CreateAsync(NCommunity obj);
        Task<NCommunity> GetAsync(int id);
        Task<IEnumerable<NCommunity>> GetAllAsync();
        Task UpdateAsync(NCommunity obj);
        Task DeleteAsync(NCommunity key);

        Task EnsureInitializedAsync();
    }
}
