using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ITeamStore
    {
        Task<int> CreateAsync(Team obj);
        Task<Team> GetAsync(int id);
        Task<IEnumerable<Team>> GetAllAsync();
        Task UpdateAsync(Team obj);
        Task DeleteAsync(Team id);

        Task EnsureInitializedAsync();
    }
}
