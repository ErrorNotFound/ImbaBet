using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SettingStore(string connectionString) : ISettingStore
    {
        private readonly string connectionString = connectionString;

        public readonly string TableName = "NSettings";

        private readonly string colName_Key = "Key";
        private readonly string colName_Value = "Value";
        private readonly string colName_Default = "Default";
        private readonly string colName_Description = "Description";

        private readonly int field_length = 128;

        public async Task CreateAsync(NSetting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_Key}],[{colName_Value}],[{colName_Default}],[{colName_Description}]) " +
                "VALUES " +
                $"(@{colName_Key},@{colName_Value},@{colName_Default},@{colName_Description});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_Key}", SqlDbType.NVarChar, field_length).Value = setting.Key;
            command.Parameters.Add($"@{colName_Value}", SqlDbType.NVarChar, field_length).Value = setting.Value;
            command.Parameters.Add($"@{colName_Default}", SqlDbType.NVarChar, field_length).Value = setting.Default;
            command.Parameters.Add($"@{colName_Description}", SqlDbType.NVarChar, field_length).Value = setting.Description;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task DeleteAsync(NSetting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Key}]=@{colName_Key}";

            command.Parameters.Add($"@{colName_Key}", SqlDbType.NVarChar, field_length).Value = setting.Key;

            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task EnsureInitializedAsync()
        {
            using var connection = new SqlConnection(connectionString);
            using var tableExistsCommand = connection.CreateCommand();
            tableExistsCommand.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @{TableName}";
            tableExistsCommand.Parameters.Add($"@{TableName}", SqlDbType.NVarChar, field_length).Value = TableName;

            await connection.OpenAsync();
            await tableExistsCommand.PrepareAsync();
            var result = await tableExistsCommand.ExecuteScalarAsync();
            await connection.CloseAsync();

            if (result == null)
                throw new InvalidOperationException("Error checking if table exists.");

            if ((int)result > 0)
            {
                // table exists
                return;
            }

            using var command = connection.CreateCommand();

            command.CommandText =
                $"CREATE TABLE {TableName} " +
                $"(" +
                    $"[{colName_Key}] NVARCHAR({field_length}) NOT NULL PRIMARY KEY, " +
                    $"[{colName_Value}] NVARCHAR({field_length}) NOT NULL, " +
                    $"[{colName_Default}] NVARCHAR({field_length}) NOT NULL, " +
                    $"[{colName_Description}] NVARCHAR({field_length}) NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<NSetting> GetAsync(string key)
        {
            var itemList = await InternalGetAsync(key);
            var count = itemList.Count();

            if (count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<NSetting>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<NSetting>> InternalGetAsync(string? key)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            if (key != null)
            {
                command.CommandText = $"SELECT * FROM {TableName} WHERE [{colName_Key}]=@{colName_Key}";
                command.Parameters.Add($"@{colName_Key}", SqlDbType.NVarChar, field_length).Value = key;
            }
            else
            {
                command.CommandText = $"SELECT * FROM {TableName}";
            }

            await connection.OpenAsync();
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();

            int oKey = reader.GetOrdinal(colName_Key);
            int oValue = reader.GetOrdinal(colName_Value);
            int oDefault = reader.GetOrdinal(colName_Default);
            int oDescription = reader.GetOrdinal(colName_Description);

            var list = new List<NSetting>();

            while (await reader.ReadAsync())
            {
                list.Add(new NSetting
                {
                    Key = reader.GetString(oKey),
                    Value = reader.GetString(oValue),
                    Default = reader.GetString(oDefault),
                    Description = reader.GetString(oDescription)
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(NSetting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_Value}]=@{colName_Value},[{colName_Default}]=@{colName_Default},[{colName_Description}]=@{colName_Description} " +
                $"WHERE [{colName_Key}]=@{colName_Key}";

            command.Parameters.Add($"@{colName_Value}", SqlDbType.NVarChar, field_length).Value = setting.Value;
            command.Parameters.Add($"@{colName_Default}", SqlDbType.NVarChar, field_length).Value = setting.Default;
            command.Parameters.Add($"@{colName_Description}", SqlDbType.NVarChar, field_length).Value = setting.Description;
            command.Parameters.Add($"@{colName_Key}", SqlDbType.NVarChar, field_length).Value = setting.Key;

            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
