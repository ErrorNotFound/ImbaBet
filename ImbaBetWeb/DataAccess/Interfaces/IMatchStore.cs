using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IMatchStore
    {
        Task<int> CreateAsync(NMatch obj);
        Task<NMatch> GetAsync(int id);
        Task<IEnumerable<NMatch>> GetAllAsync();
        Task UpdateAsync(NMatch obj);
        Task DeleteAsync(NMatch id);

        Task EnsureInitializedAsync();
    }
}
