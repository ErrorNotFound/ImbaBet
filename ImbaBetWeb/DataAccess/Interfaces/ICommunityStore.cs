using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ICommunityStore
    {
        Task CreateAsync(NCommunity obj);
        Task<NCommunity> GetAsync(string id);
        Task<IEnumerable<NCommunity>> GetAllAsync();
        Task UpdateAsync(NCommunity obj);
        Task DeleteAsync(NCommunity key);

        Task EnsureInitializedAsync();
    }
}
