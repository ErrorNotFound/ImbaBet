using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlSettingStore(string connectionString) : SqlStoreBase<Setting, string>(), ISettingStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.NVarChar, 256, true);
        private readonly SqlColumnDefinition columnValue = new("Value", SqlDbType.NVarChar, 256);
        private readonly SqlColumnDefinition columnDefault = new("Default", SqlDbType.NVarChar, 256);
        private readonly SqlColumnDefinition columnDescription = new("Description", SqlDbType.NVarChar, 256);

        public override string TableName => "Settings";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Setting source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnValue, source.Value },
                { columnDefault, source.Default },
                { columnDescription, source.Description }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Setting, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Setting, dynamic?>>()
            {
                { columnId, (setting, valueToBeSet) => { setting.Id = valueToBeSet!; } },
                { columnValue, (setting, valueToBeSet) => { setting.Value = valueToBeSet!; } },
                { columnDefault, (setting, valueToBeSet) => { setting.Default = valueToBeSet!; } },
                { columnDescription, (setting, valueToBeSet) => { setting.Description = valueToBeSet!; } }
            };
        }

        public override Task<string> CreateAsync(Setting item)
        {
            return CreateAsync(item, true);
        }
    }
}
