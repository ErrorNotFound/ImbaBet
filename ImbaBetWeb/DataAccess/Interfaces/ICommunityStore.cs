using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ICommunityStore
    {
        Task<int> CreateAsync(Community obj);
        Task<Community> GetAsync(int id);
        Task<IEnumerable<Community>> GetAllAsync();
        Task UpdateAsync(Community obj);
        Task DeleteAsync(Community key);

        Task EnsureInitializedAsync();
    }
}
