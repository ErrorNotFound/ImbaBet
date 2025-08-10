using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IBetStore
    {
        Task<int> CreateAsync(Bet obj);
        Task<Bet> GetAsync(int id);
        Task<IEnumerable<Bet>> GetAllAsync();
        Task UpdateAsync(Bet obj);
        Task DeleteAsync(Bet id);

        Task EnsureInitializedAsync();
    }
}
