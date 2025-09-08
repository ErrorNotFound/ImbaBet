using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.Tests.Unit.TestHelper
{
    public class TestStore<T, U> : IStoreBase<T, U> where T : IIdentifiable<U>, new() where U : notnull
    {
        public List<T> Items { get; } = [];

        public Task<U> CreateAsync(T obj)
        {
            Items.Add(obj);
            return Task.FromResult(obj.Id);
        }

        public Task DeleteAllAsync()
        {
            Items.Clear();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T id)
        {
            Items.RemoveAll(i => i.Id.Equals(id.Id));
            return Task.CompletedTask;
        }

        public Task EnsureInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Items.AsEnumerable());
        }

        public Task<T> GetAsync(U id)
        {
            return Task.FromResult(Items.Single(i => i.Id.Equals(id)));
        }

        public Task UpdateAsync(T obj)
        {
            Items.RemoveAll(i => i.Id.Equals(obj.Id));
            Items.Add(obj);
            return Task.CompletedTask;
        }
    }
}
