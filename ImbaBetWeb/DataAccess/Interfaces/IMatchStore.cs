using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IMatchStore
    {
        Task<int> CreateAsync(Match obj);
        Task<Match> GetAsync(int id);
        Task<IEnumerable<Match>> GetAllAsync();
        Task UpdateAsync(Match obj);
        Task DeleteAsync(Match id);

        Task EnsureInitializedAsync();
    }
}
