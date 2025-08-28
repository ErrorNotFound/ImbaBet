using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SqlTeamStore(string connectionString) : ITeamStore
    {
        private readonly string connectionString = connectionString;
        
        public readonly string TableName = "Teams";

        private readonly string colName_Id = "Id";
        private readonly string colName_Name = "Name";
        private readonly string colName_FlagCountryCode = "FlagCountryCode";
        private readonly string colName_StackRank = "StackRank";

        private readonly int field_length = 128;

        public async Task<int> CreateAsync(Team team)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_Name}],[{colName_FlagCountryCode}],[{colName_StackRank}]) " +
                "VALUES " +
                $"(@{colName_Name},@{colName_FlagCountryCode},@{colName_StackRank});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_Name}", SqlDbType.NVarChar, field_length).Value = team.Name;
            command.Parameters.Add($"@{colName_FlagCountryCode}", SqlDbType.NVarChar, field_length).Value = team.FlagCountryCode ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_StackRank}", SqlDbType.Int).Value = team.StackRank;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result == null)
                throw new InvalidOperationException("Error while creating new item. Could not get primary key.");
            return (int)result;
        }

        public async Task DeleteAsync(Team team)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = team.Id;

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
                    $"[{colName_Id}] int NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                    $"[{colName_Name}] NVARCHAR({field_length}) NOT NULL, " +
                    $"[{colName_FlagCountryCode}] NVARCHAR({field_length}), " +
                    $"[{colName_StackRank}] int NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<Team> GetAsync(int id)
        {
            var itemList = await InternalGetAsync(id);
            var count = itemList.Count();

            if(count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<Team>> InternalGetAsync(int? id)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            if(id != null)
            {
                command.CommandText = $"SELECT * FROM {TableName} WHERE [{colName_Id}]=@{colName_Id}";
                command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = id;  
            }
            else
            {
                command.CommandText = $"SELECT * FROM {TableName}";
            }

            await connection.OpenAsync();
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();

            int oId = reader.GetOrdinal(colName_Id);
            int oName = reader.GetOrdinal(colName_Name);
            int oFlagCountryCode = reader.GetOrdinal(colName_FlagCountryCode);
            int oStackRank = reader.GetOrdinal(colName_StackRank);

            var list = new List<Team>();

            while (await reader.ReadAsync())
            {
                list.Add(new Team
                {
                    Id = reader.GetInt32(oId),
                    Name = reader.GetString(oName),
                    FlagCountryCode = reader.IsDBNull(oFlagCountryCode) ? null : reader.GetString(oFlagCountryCode),
                    StackRank = reader.GetInt32(oStackRank)
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(Team team)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_Name}]=@{colName_Name},[{colName_FlagCountryCode}]=@{colName_FlagCountryCode},[{colName_StackRank}]=@{colName_StackRank} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Name}", SqlDbType.NVarChar, field_length).Value = team.Name;
            command.Parameters.Add($"@{colName_FlagCountryCode}", SqlDbType.NVarChar, field_length).Value = team.FlagCountryCode ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_StackRank}", SqlDbType.Int).Value = team.StackRank;
            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = team.Id;
            
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
