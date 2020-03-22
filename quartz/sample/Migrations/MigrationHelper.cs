using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace TysonFury
{
    public class MigrationHelper
    {
        /// <summary>
        /// Применяет миграции БД.pel
        /// </summary>
        public void ApplyDatabaseMigrations(string connectionString, IMigrationRunner runner, ILogger<MigrationHelper> logger)
        {
            try
            {
                if (!CanOpenConnection(connectionString))
                    CreateDatabase(connectionString, logger);
                
                TryOpenConnection(connectionString);
                runner.MigrateUp();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Ошибка при применении миграций БД");
                throw;
            }
        }
        

        private static void CreateDatabase(string connectionString, ILogger logger)
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = connectionString };

            var databaseName = builder.Database;
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                var secured = SecureConnectionString(connectionString);
                var message = $"Missing database name in connection string (ConnectionString={secured})";
                throw new InvalidOperationException(message);
            }

            builder.Database = "postgres";
            var systemDatabaseConnectionString = builder.ConnectionString;

            using (var connection = new NpgsqlConnection(systemDatabaseConnectionString))
            {
                connection.Open();

                if (DatabaseExists(databaseName, connection, logger))
                {
                    return;
                }

                var sql = $"CREATE DATABASE \"{databaseName}\" ENCODING = DEFAULT CONNECTION LIMIT = -1";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    logger?.LogInformation($"Execute SQL command: {sql}");
                    command.ExecuteScalar();
                }
            }
        }

        private static string SecureConnectionString(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = connectionString };
            if (!string.IsNullOrWhiteSpace(builder.Password))
                builder.Password = "*****";
            return builder.ConnectionString;
        }

        private static bool CanOpenConnection(string connectionString)
        {
            try
            {
                TryOpenConnection(connectionString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void TryOpenConnection(string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
            }
        }

        private static bool DatabaseExists(string databaseName, NpgsqlConnection connection, ILogger logger)
        {
            const string sql = "SELECT datname FROM pg_catalog.pg_database WHERE datname = @databaseName";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("databaseName", databaseName);

                logger?.LogInformation($"Execute SQL command: {sql} with parameter databaseName={databaseName}");
                var result = command.ExecuteScalar();
                logger?.LogInformation($"Database {databaseName} exists={result != null}");

                return result != null;
            }
        }
    }
}