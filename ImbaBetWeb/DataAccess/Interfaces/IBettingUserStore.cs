using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IBettingUserStore
    {
        Task<int> CreateAsync(NBettingUser obj);
        Task<NBettingUser> GetAsync(int id);
        Task<IEnumerable<NBettingUser>> GetAllAsync();
        Task UpdateAsync(NBettingUser obj);
        Task DeleteAsync(NBettingUser id);

        Task EnsureInitializedAsync();
    }
}
