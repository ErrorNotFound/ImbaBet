using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class BetStore(string connectionString) : IBetStore
    {
        private readonly string connectionString = connectionString;
        
        public readonly string TableName = "Bets";

        private readonly string colName_Id = "Id";
        private readonly string colName_MatchId = "MatchId";
        private readonly string colName_UserId = "UserId";
        private readonly string colName_GoalsA = "GoalsA";
        private readonly string colName_GoalsB = "GoalsB";
        private readonly string colName_Points = "Points";

        private readonly int field_length = 128;

        public async Task<int> CreateAsync(Bet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_MatchId}],[{colName_UserId}],[{colName_GoalsA}],[{colName_GoalsB}],[{colName_Points}]) " +
                "VALUES " +
                $"(@{colName_MatchId},@{colName_UserId},@{colName_GoalsA},@{colName_GoalsB},@{colName_Points});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_MatchId}", SqlDbType.Int).Value = bet.MatchId;
            command.Parameters.Add($"@{colName_UserId}", SqlDbType.Int).Value = bet.UserId;
            command.Parameters.Add($"@{colName_GoalsA}", SqlDbType.Int).Value = bet.GoalsA;
            command.Parameters.Add($"@{colName_GoalsB}", SqlDbType.Int).Value = bet.GoalsB;
            command.Parameters.Add($"@{colName_Points}", SqlDbType.Int).Value = bet.Points;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result == null)
                throw new InvalidOperationException("Error while creating new item. Could not get primary key.");
            return (int)result;
        }

        public async Task DeleteAsync(Bet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = bet.Id;

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
                    $"[{colName_MatchId}] int NOT NULL, " +
                    $"[{colName_UserId}] int NOT NULL, " +
                    $"[{colName_GoalsA}] int NOT NULL, " +
                    $"[{colName_GoalsB}] int NOT NULL, " +
                    $"[{colName_Points}] int NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<Bet> GetAsync(int id)
        {
            var itemList = await InternalGetAsync(id);
            var count = itemList.Count();

            if(count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<Bet>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<Bet>> InternalGetAsync(int? id)
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
            int oMatchId = reader.GetOrdinal(colName_MatchId);
            int oUserId = reader.GetOrdinal(colName_UserId);
            int oGoalA = reader.GetOrdinal(colName_GoalsA);
            int oGoalB = reader.GetOrdinal(colName_GoalsB);
            int oPoints = reader.GetOrdinal(colName_Points);

            var list = new List<Bet>();

            while (await reader.ReadAsync())
            {
                list.Add(new Bet
                {
                    Id = reader.GetInt32(oId),
                    MatchId = reader.GetInt32(oMatchId),
                    UserId = reader.GetInt32(oUserId),
                    GoalsA = reader.GetInt32(oGoalA),
                    GoalsB = reader.GetInt32(oGoalB),
                    Points = reader.GetInt32(oPoints)
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(Bet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_MatchId}]=@{colName_MatchId},[{colName_UserId}]=@{colName_UserId},[{colName_GoalsA}]=@{colName_GoalsA},[{colName_GoalsB}]=@{colName_GoalsB},[{colName_Points}]=@{colName_Points} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_MatchId}", SqlDbType.Int).Value = bet.MatchId;
            command.Parameters.Add($"@{colName_UserId}", SqlDbType.Int).Value = bet.UserId;
            command.Parameters.Add($"@{colName_GoalsA}", SqlDbType.Int).Value = bet.GoalsA;
            command.Parameters.Add($"@{colName_GoalsB}", SqlDbType.Int).Value = bet.GoalsB;
            command.Parameters.Add($"@{colName_Points}", SqlDbType.Int).Value = bet.Points;
            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = bet.Id;
            
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
