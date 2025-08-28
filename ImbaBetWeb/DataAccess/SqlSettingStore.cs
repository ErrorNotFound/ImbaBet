using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SqlSettingStore(string connectionString) : ISettingStore
    {
        private readonly string connectionString = connectionString;

        public readonly string TableName = "Settings";

        private readonly string colName_Id = "Id";
        private readonly string colName_Value = "Value";
        private readonly string colName_Default = "Default";
        private readonly string colName_Description = "Description";

        private readonly int field_length = 128;

        public async Task<string> CreateAsync(Setting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_Id}],[{colName_Value}],[{colName_Default}],[{colName_Description}]) " +
                "VALUES " +
                $"(@{colName_Id},@{colName_Value},@{colName_Default},@{colName_Description});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.NVarChar, field_length).Value = setting.Id;
            command.Parameters.Add($"@{colName_Value}", SqlDbType.NVarChar, field_length).Value = setting.Value;
            command.Parameters.Add($"@{colName_Default}", SqlDbType.NVarChar, field_length).Value = setting.Default;
            command.Parameters.Add($"@{colName_Description}", SqlDbType.NVarChar, field_length).Value = setting.Description;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();

            return setting.Id;
        }

        public async Task DeleteAsync(Setting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.NVarChar, field_length).Value = setting.Id;

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
                    $"[{colName_Id}] NVARCHAR({field_length}) NOT NULL PRIMARY KEY, " +
                    $"[{colName_Value}] NVARCHAR({field_length}) NOT NULL, " +
                    $"[{colName_Default}] NVARCHAR({field_length}) NOT NULL, " +
                    $"[{colName_Description}] NVARCHAR({field_length}) NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<Setting> GetAsync(string id)
        {
            var itemList = await InternalGetAsync(id);
            var count = itemList.Count();

            if (count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<Setting>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<Setting>> InternalGetAsync(string? id)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            if (id != null)
            {
                command.CommandText = $"SELECT * FROM {TableName} WHERE [{colName_Id}]=@{colName_Id}";
                command.Parameters.Add($"@{colName_Id}", SqlDbType.NVarChar, field_length).Value = id;
            }
            else
            {
                command.CommandText = $"SELECT * FROM {TableName}";
            }

            await connection.OpenAsync();
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();

            int oId = reader.GetOrdinal(colName_Id);
            int oValue = reader.GetOrdinal(colName_Value);
            int oDefault = reader.GetOrdinal(colName_Default);
            int oDescription = reader.GetOrdinal(colName_Description);

            var list = new List<Setting>();

            while (await reader.ReadAsync())
            {
                list.Add(new Setting
                {
                    Id = reader.GetString(oId),
                    Value = reader.GetString(oValue),
                    Default = reader.GetString(oDefault),
                    Description = reader.GetString(oDescription)
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(Setting setting)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_Value}]=@{colName_Value},[{colName_Default}]=@{colName_Default},[{colName_Description}]=@{colName_Description} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Value}", SqlDbType.NVarChar, field_length).Value = setting.Value;
            command.Parameters.Add($"@{colName_Default}", SqlDbType.NVarChar, field_length).Value = setting.Default;
            command.Parameters.Add($"@{colName_Description}", SqlDbType.NVarChar, field_length).Value = setting.Description;
            command.Parameters.Add($"@{colName_Id}", SqlDbType.NVarChar, field_length).Value = setting.Id;

            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task DeleteAllAsync()
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"DELETE FROM {TableName}";

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
