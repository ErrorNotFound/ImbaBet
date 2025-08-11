using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ICommunityStore
    {
        Task<int> CreateAsync(Community community);
        Task<Community> GetAsync(int id);
        Task<IEnumerable<Community>> GetAllAsync();
        Task UpdateAsync(Community community);
        Task DeleteAsync(Community community);

        Task EnsureInitializedAsync();
    }
}
