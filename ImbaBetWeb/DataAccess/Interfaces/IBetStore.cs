using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IBetStore
    {
        Task<int> CreateAsync(NBet obj);
        Task<NBet> GetAsync(int id);
        Task<IEnumerable<NBet>> GetAllAsync();
        Task UpdateAsync(NBet obj);
        Task DeleteAsync(NBet id);

        Task EnsureInitializedAsync();
    }
}
