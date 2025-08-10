using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IBettingUserStore
    {
        Task<int> CreateAsync(BettingUser obj);
        Task<BettingUser> GetAsync(int id);
        Task<IEnumerable<BettingUser>> GetAllAsync();
        Task UpdateAsync(BettingUser obj);
        Task DeleteAsync(BettingUser id);

        Task EnsureInitializedAsync();
    }
}
