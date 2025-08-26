using ImbaBetWeb.Model;

namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface IStoreBase<T,U> where T : IIdentifiable<U>
    {
        Task<U> CreateAsync(T obj);
        Task<T> GetAsync(U id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T obj);
        Task DeleteAsync(T id);

        Task EnsureInitializedAsync();
    }
}
