using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ISettingStore
    {
        Task CreateAsync(NSetting obj);
        Task<NSetting> GetAsync(string key);
        Task<IEnumerable<NSetting>> GetAllAsync();
        Task UpdateAsync(NSetting obj);
        Task DeleteAsync(NSetting key);

        Task EnsureInitializedAsync();
    }
}
