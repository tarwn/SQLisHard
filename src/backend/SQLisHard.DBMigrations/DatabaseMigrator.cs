using DbUp;
using Microsoft.Data.SqlClient;
using SQLisHard.DBMigrations.Utilities;
using SQLisHard.DBMigrations.Utilities.Logging;
using System.Reflection;

namespace SQLisHard.DBMigrations
{
    public class DatabaseMigrator
    {
       internal static List<LogEntry> UpgradeWithLog(string connectionString, string migrationsFolderPath)
        {
            var logger = new StringLogger();

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(migrationsFolderPath)
                .WithTransaction()
                .LogTo(logger)
                .LogScriptOutput()
                .Build();

            var sql = new SqlConnection(connectionString);
            var databaseName = sql.Database;

            string error;
            if (!upgrader.TryConnect(out error))
            {
                logger.WriteCriticalError($"Database Error: could not connect to '${databaseName}': ${error}");
                return logger.GetLog();
            }

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                logger.WriteCriticalError($"Database upgrade for '${databaseName}' failed with error: {result.Error}");
            }

            return logger.GetLog();
        }

        public static void UpgradeWithExceptions(string connectionString, string migrationsFolderPath)
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(migrationsFolderPath)
                .WithTransaction()
                .LogToConsole()
                .LogScriptOutput()
                .Build();

            string error;
            if (!upgrader.TryConnect(out error))
            {
                throw new Exception($"Database Error: could not connect to the test database provided: ${error}");
            }

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new MigrationException($"Database upgrade failed with error: {result.Error}", result);
            }
        }
    }
}
