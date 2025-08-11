using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class BettingUserStore(string connectionString) : IBettingUserStore
    {
        private readonly string connectionString = connectionString;
        
        public readonly string TableName = "BettingUsers";

        private readonly string colName_Id = "Id";
        private readonly string colName_MemberOfCommunityId = "MemberOfCommunityId";
        private readonly string colName_Points = "Points";

        private readonly int field_length = 128;

        public async Task<int> CreateAsync(BettingUser user)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_MemberOfCommunityId}],[{colName_Points}]) " +
                "VALUES " +
                $"(@{colName_MemberOfCommunityId},@{colName_Points});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_MemberOfCommunityId}", SqlDbType.Int).Value = user.MemberOfCommunityId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_Points}", SqlDbType.Int).Value = user.Points;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result == null)
                throw new InvalidOperationException("Error while creating new item. Could not get primary key.");
            return (int)result;
        }

        public async Task DeleteAsync(BettingUser user)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = user.Id;

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
                    $"[{colName_MemberOfCommunityId}] int, " +
                    $"[{colName_Points}] int NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<BettingUser> GetAsync(int id)
        {
            var itemList = await InternalGetAsync(id);
            var count = itemList.Count();

            if(count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<BettingUser>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<BettingUser>> InternalGetAsync(int? id)
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
            int oMemberOfCommunityId = reader.GetOrdinal(colName_MemberOfCommunityId);
            int oPoints = reader.GetOrdinal(colName_Points);

            var list = new List<BettingUser>();

            while (await reader.ReadAsync())
            {
                list.Add(new BettingUser
                {
                    Id = reader.GetInt32(oId),
                    MemberOfCommunityId = reader.IsDBNull(oMemberOfCommunityId) ? null : reader.GetInt32(oMemberOfCommunityId),
                    Points = reader.GetInt32(oPoints)
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(BettingUser user)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_MemberOfCommunityId}]=@{colName_MemberOfCommunityId},[{colName_Points}]=@{colName_Points} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_MemberOfCommunityId}", SqlDbType.Int).Value = user.MemberOfCommunityId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_Points}", SqlDbType.Int).Value = user.Points;
            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = user.Id;
            
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
