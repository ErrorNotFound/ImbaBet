using Microsoft.Data.SqlClient;

namespace ImbaBetWeb.Test.DataAccess.TestHelper
{
    internal class SqlTestDatabase
    {
        public string ConnectionString => $"{DataSource};Initial Catalog={TestDatabaseName};";
        private string DataSource = "Data Source=(localdb)\\MSSQLLocalDB";
        private string TestDatabaseName = "TestDatabase";

        public async Task PrepareAsync()
        {
            await DeleteDatabaseAsync();
            await CreateDatabaseIfNotExistsAsync();
        }

        public async Task FinalizeAsync()
        {
            await DeleteDatabaseAsync();
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @{tableName}";
            command.Parameters.Add($"@{tableName}", System.Data.SqlDbType.NVarChar, 128).Value = tableName;

            await connection.OpenAsync();
            await command.PrepareAsync();
            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();

            if (result == null)
                throw new InvalidOperationException("Error checking if table exists.");

            return (int)result > 0 ? true : false;
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            using var connection = new SqlConnection(DataSource);
            using var cmd = new SqlCommand($"If(db_id(N'{TestDatabaseName}') IS NULL) CREATE DATABASE [{TestDatabaseName}]", connection);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }

        private async Task DeleteDatabaseAsync()
        {
            using var connection = new SqlConnection(DataSource);
            using var cmd = new SqlCommand($"If(db_id(N'{TestDatabaseName}') IS NOT NULL)BEGIN ALTER DATABASE {TestDatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE [{TestDatabaseName}] END;", connection);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await connection.CloseAsync();
        }
    }
}
