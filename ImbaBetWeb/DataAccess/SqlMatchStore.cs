using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImbaBetWeb.DataAccess
{
    public class SqlMatchStore(string connectionString) : IMatchStore
    {
        private readonly string connectionString = connectionString;
        
        public readonly string TableName = "Matches";

        private readonly string colName_Id = "Id";
        private readonly string colName_DateTime = "DateTime";
        private readonly string colName_TeamATeamId = "TeamATeamId";
        private readonly string colName_TeamBTeamId = "TeamBTeamId";
        private readonly string colName_AlternativeTeamAText = "AlternativeTeamAText";
        private readonly string colName_AlternativeTeamBText = "AlternativeTeamBText";
        private readonly string colName_GoalsA = "GoalsA";
        private readonly string colName_GoalsB = "GoalsB";
        private readonly string colName_IsOver = "IsOver";
        private readonly string colName_MatchGroupId = "MatchGroupId";

        private readonly int field_length = 128;

        public async Task<int> CreateAsync(Match match)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"INSERT INTO {TableName} " +
                $"([{colName_DateTime}],[{colName_TeamATeamId}],[{colName_TeamBTeamId}],[{colName_AlternativeTeamAText}],[{colName_AlternativeTeamBText}],[{colName_GoalsA}],[{colName_GoalsB}],[{colName_IsOver}],[{colName_MatchGroupId}]) " +
                "VALUES " +
                $"(@{colName_DateTime},@{colName_TeamATeamId},@{colName_TeamBTeamId},@{colName_AlternativeTeamAText},@{colName_AlternativeTeamBText},@{colName_GoalsA},@{colName_GoalsB},@{colName_IsOver},@{colName_MatchGroupId});" +
                $"SELECT CONVERT(int,SCOPE_IDENTITY());";

            command.Parameters.Add($"@{colName_DateTime}", SqlDbType.DateTime2, field_length).Value = match.DateTime; // DateTime2 requires a length, but has no impact
            command.Parameters.Add($"@{colName_TeamATeamId}", SqlDbType.Int).Value = match.TeamATeamId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_TeamBTeamId}", SqlDbType.Int).Value = match.TeamBTeamId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_AlternativeTeamAText}", SqlDbType.NVarChar, field_length).Value = match.AlternativeTeamAText ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_AlternativeTeamBText}", SqlDbType.NVarChar, field_length).Value = match.AlternativeTeamBText ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_GoalsA}", SqlDbType.Int).Value = match.GoalsA;
            command.Parameters.Add($"@{colName_GoalsB}", SqlDbType.Int).Value = match.GoalsB;
            command.Parameters.Add($"@{colName_IsOver}", SqlDbType.Bit).Value = match.IsOver;
            command.Parameters.Add($"@{colName_MatchGroupId}", SqlDbType.Int).Value = match.MatchGroupId;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();
            if (result == null)
                throw new InvalidOperationException("Error while creating new item. Could not get primary key.");
            return (int)result;
        }

        public async Task DeleteAsync(Match match)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText =
                $"DELETE FROM {TableName} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = match.Id;

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
                    $"[{colName_DateTime}] DateTime2 NOT NULL, " +
                    $"[{colName_TeamATeamId}] int, " +
                    $"[{colName_TeamBTeamId}] int, " +
                    $"[{colName_AlternativeTeamAText}] NVARCHAR({field_length}), " +
                    $"[{colName_AlternativeTeamBText}] NVARCHAR({field_length}), " +
                    $"[{colName_GoalsA}] int NOT NULL, " +
                    $"[{colName_GoalsB}] int NOT NULL, " +
                    $"[{colName_IsOver}] bit NOT NULL, " +
                    $"[{colName_MatchGroupId}] int NOT NULL " +
                ")";
            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        public async Task<Match> GetAsync(int id)
        {
            var itemList = await InternalGetAsync(id);
            var count = itemList.Count();

            if(count == 0)
                throw new InvalidOperationException("No records were returned.");

            if (count > 1)
                throw new InvalidOperationException("Multiple records were returned.");

            return itemList.Single();
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await InternalGetAsync(null);
        }

        private async Task<IEnumerable<Match>> InternalGetAsync(int? id)
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
            int oDateTime = reader.GetOrdinal(colName_DateTime);
            int oTeamATeamId = reader.GetOrdinal(colName_TeamATeamId);
            int oTeamBTeamId = reader.GetOrdinal(colName_TeamBTeamId);
            int oAlternativeTeamAText = reader.GetOrdinal(colName_AlternativeTeamAText);
            int oAlternativeTeamBText = reader.GetOrdinal(colName_AlternativeTeamBText);
            int oGoalsA = reader.GetOrdinal(colName_GoalsA);
            int oGoalsB = reader.GetOrdinal(colName_GoalsB);
            int oIsOver = reader.GetOrdinal(colName_IsOver);
            int oMatchGroupId = reader.GetOrdinal(colName_MatchGroupId);

            var list = new List<Match>();

            while (await reader.ReadAsync())
            {
                list.Add(new Match
                {
                    Id = reader.GetInt32(oId),
                    DateTime = reader.GetDateTime(oDateTime),
                    TeamATeamId = reader.IsDBNull(oTeamATeamId) ? null : reader.GetInt32(oTeamATeamId),
                    TeamBTeamId = reader.IsDBNull(oTeamBTeamId) ? null : reader.GetInt32(oTeamBTeamId),
                    AlternativeTeamAText = reader.IsDBNull(oAlternativeTeamAText) ? null : reader.GetString(oAlternativeTeamAText),
                    AlternativeTeamBText = reader.IsDBNull(oAlternativeTeamBText) ? null : reader.GetString(oAlternativeTeamBText),
                    GoalsA = reader.GetInt32(oGoalsA),
                    GoalsB = reader.GetInt32(oGoalsB),
                    IsOver = reader.GetBoolean(oIsOver),
                    MatchGroupId = reader.GetInt32(oMatchGroupId),
                });
            }

            await connection.CloseAsync();
            return list;
        }

        public async Task UpdateAsync(Match match)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = $"UPDATE {TableName} " +
                $"SET [{colName_DateTime}]=@{colName_DateTime},[{colName_TeamATeamId}]=@{colName_TeamATeamId},[{colName_TeamBTeamId}]=@{colName_TeamBTeamId},[{colName_AlternativeTeamAText}]=@{colName_AlternativeTeamAText},[{colName_AlternativeTeamBText}]=@{colName_AlternativeTeamBText},[{colName_GoalsA}]=@{colName_GoalsA},[{colName_GoalsB}]=@{colName_GoalsB},[{colName_IsOver}]=@{colName_IsOver},[{colName_MatchGroupId}]=@{colName_MatchGroupId} " +
                $"WHERE [{colName_Id}]=@{colName_Id}";

            command.Parameters.Add($"@{colName_DateTime}", SqlDbType.DateTime2, field_length).Value = match.DateTime; // DateTime2 requires a length, but has no impact
            command.Parameters.Add($"@{colName_TeamATeamId}", SqlDbType.Int).Value = match.TeamATeamId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_TeamBTeamId}", SqlDbType.Int).Value = match.TeamBTeamId ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_AlternativeTeamAText}", SqlDbType.NVarChar, field_length).Value = match.AlternativeTeamAText ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_AlternativeTeamBText}", SqlDbType.NVarChar, field_length).Value = match.AlternativeTeamBText ?? (object)DBNull.Value;
            command.Parameters.Add($"@{colName_GoalsA}", SqlDbType.Int).Value = match.GoalsA;
            command.Parameters.Add($"@{colName_GoalsB}", SqlDbType.Int).Value = match.GoalsB;
            command.Parameters.Add($"@{colName_IsOver}", SqlDbType.Bit).Value = match.IsOver;
            command.Parameters.Add($"@{colName_MatchGroupId}", SqlDbType.Int).Value = match.MatchGroupId;
            command.Parameters.Add($"@{colName_Id}", SqlDbType.Int).Value = match.Id;

            await connection.OpenAsync();
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
