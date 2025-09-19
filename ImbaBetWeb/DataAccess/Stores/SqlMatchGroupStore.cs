using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlMatchGroupStore(string connectionString) : SqlStoreBase<MatchGroup, int>(), IMatchGroupStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnName = new("Name", SqlDbType.NVarChar, 256);
        private readonly SqlColumnDefinition columnHasGroupRanking = new("HasGroupRanking", SqlDbType.Bit);
        private readonly SqlColumnDefinition columnStackRank = new("StackRank", SqlDbType.Int);

        public override string TableName => "MatchGroups";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(MatchGroup source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnName, source.Name },
                { columnHasGroupRanking, source.HasGroupRanking },
                { columnStackRank, source.StackRank }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<MatchGroup, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<MatchGroup, dynamic?>>()
            {
                { columnId, (matchGroup, valueToBeSet) => { matchGroup.Id = valueToBeSet; } },
                { columnName, (matchGroup, valueToBeSet) => { matchGroup.Name = valueToBeSet!; } },
                { columnHasGroupRanking, (matchGroup, valueToBeSet) => { matchGroup.HasGroupRanking = valueToBeSet; } },
                { columnStackRank, (matchGroup, valueToBeSet) => { matchGroup.StackRank = valueToBeSet; } }
            };
        }
    }
}
