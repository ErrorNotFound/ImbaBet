using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SqlCommunityStore(string connectionString) : SqlStoreBase<Community, int>(), ICommunityStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnName = new("Name", SqlDbType.NVarChar, 256);
        private readonly SqlColumnDefinition columnOwnerId = new("OwnerId", SqlDbType.Int);

        public override string TableName => "Communities";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Community source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnName, source.Name },
                { columnOwnerId, source.OwnerId }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Community, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Community, dynamic?>>()
            {
                { columnId, (community, valueToBeSet) => { community.Id = valueToBeSet; } },
                { columnName, (community, valueToBeSet) => { community.Name = valueToBeSet!; } },
                { columnOwnerId, (community, valueToBeSet) => { community.OwnerId = valueToBeSet; } }
            };
        }
    }
}
