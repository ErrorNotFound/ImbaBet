namespace ImbaBetWeb.DataAccess
{
    public interface ICrud<T>
    {
        Task<int> CreateAsync(T obj);
        Task<T> RetrieveAsync(int key);
        Task UpdateAsync(T obj);
        Task DeleteAsync(T key);

        Task EnsureInitializedAsync();
    }
}
