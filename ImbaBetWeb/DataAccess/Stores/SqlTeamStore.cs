using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlTeamStore(string connectionString) : SqlStoreBase<Team, int>(), ITeamStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnName = new("Name", SqlDbType.NVarChar, 256);
        private readonly SqlColumnDefinition columnFlagCountryCode = new("FlagCountryCode", SqlDbType.NVarChar, 256, false, true);
        private readonly SqlColumnDefinition columnStackRank = new("StackRank", SqlDbType.Int);

        public override string TableName => "Teams";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Team source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnName, source.Name },
                { columnFlagCountryCode, source.FlagCountryCode },
                { columnStackRank, source.StackRank }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Team, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Team, dynamic?>>()
            {
                { columnId, (team, valueToBeSet) => { team.Id = valueToBeSet; } },
                { columnName, (team, valueToBeSet) => { team.Name = valueToBeSet!; } },
                { columnFlagCountryCode, (team, valueToBeSet) => { team.FlagCountryCode = valueToBeSet; } },
                { columnStackRank, (team, valueToBeSet) => { team.StackRank = valueToBeSet; } }
            };
        }
    }
}
