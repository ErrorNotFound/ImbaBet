using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public interface IBetStore : ICrud<NBet>
    {
    }

    public class BetStore(string connectionString) : IBetStore
    {
        private readonly string connectionString = connectionString;
        
        public readonly string TableName = "NBets";

        private readonly string colName_Id = "Id";
        private readonly string colName_MatchId = "MatchId";
        private readonly string colName_UserId = "UserId";
        private readonly string colName_GoalsA = "GoalsA";
        private readonly string colName_GoalsB = "GoalsB";
        private readonly string colName_Points = "Points";

        public async Task<int> CreateAsync(NBet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"({colName_MatchId},{colName_UserId},{colName_GoalsA},{colName_GoalsB},{colName_Points}) " +
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

        public async Task DeleteAsync(NBet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE {colName_Id}=@{colName_Id}";

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
            tableExistsCommand.Parameters.Add($"@{TableName}", SqlDbType.NVarChar, 128).Value = TableName;

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
                    $"{colName_Id} int NOT NULL IDENTITY(1,1) PRIMARY KEY, " +
                    $"{colName_MatchId} int NOT NULL, " +
                    $"{colName_UserId} int NOT NULL, " +
                    $"{colName_GoalsA} int NOT NULL, " +
                    $"{colName_GoalsB} int NOT NULL, " +
                    $"{colName_Points} int NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<NBet> RetrieveAsync(int key)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"SELECT * FROM {TableName} " +
                $"WHERE {colName_Id}=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = key;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var reader = await command.ExecuteReaderAsync();
 
            if (!reader.Read())
                throw new InvalidOperationException("No records were returned.");

            int id = reader.GetOrdinal(colName_Id);
            int matchId = reader.GetOrdinal(colName_MatchId);
            int userId = reader.GetOrdinal(colName_UserId);
            int goalA = reader.GetOrdinal(colName_GoalsA);
            int goalB = reader.GetOrdinal(colName_GoalsB);
            int points = reader.GetOrdinal(colName_Points);

            var bet = new NBet
            {
                Id = reader.GetInt32(id),
                MatchId = reader.GetInt32(matchId),
                UserId = reader.GetInt32(userId),
                GoalsA = reader.GetInt32(goalA),
                GoalsB = reader.GetInt32(goalB),
                Points = reader.GetInt32(points)
            };

            if (reader.Read())
                throw new InvalidOperationException("Multiple records were returned.");
            await connection.CloseAsync();
            return bet;
        }

        public async Task UpdateAsync(NBet bet)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET {colName_MatchId}=@{colName_MatchId},{colName_UserId}=@{colName_UserId},{colName_GoalsA}=@{colName_GoalsA},{colName_GoalsB}=@{colName_GoalsB},{colName_Points}=@{colName_Points} " +
                $"WHERE {colName_Id}=@{colName_Id}";

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
