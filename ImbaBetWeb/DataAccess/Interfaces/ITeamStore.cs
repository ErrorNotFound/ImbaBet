using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ITeamStore
    {
        Task<int> CreateAsync(NTeam obj);
        Task<NTeam> GetAsync(int id);
        Task<IEnumerable<NTeam>> GetAllAsync();
        Task UpdateAsync(NTeam obj);
        Task DeleteAsync(NTeam id);

        Task EnsureInitializedAsync();
    }
}
