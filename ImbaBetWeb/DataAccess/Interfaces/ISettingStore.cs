using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ISettingStore
    {
        Task CreateAsync(Setting obj);
        Task<Setting> GetAsync(string key);
        Task<IEnumerable<Setting>> GetAllAsync();
        Task UpdateAsync(Setting obj);
        Task DeleteAsync(Setting key);

        Task EnsureInitializedAsync();
    }
}
