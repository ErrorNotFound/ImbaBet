namespace ImbaBetWeb.DataAccess.Interfaces
{
    public interface ICrud<T>
    {
        Task<int> CreateAsync(T obj);
        Task<T> GetAsync(int key);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T obj);
        Task DeleteAsync(T key);

        Task EnsureInitializedAsync();
    }
}
