using SQLisHard.DBMigrations;

namespace SQLisHard.Configuration
{
    public class LocalDevelopmentTasks
    {
        internal static void MigrateDatabase(string connectionString, string migrationsPath)
        {
            LocalDatabaseMigrator.Execute(connectionString, migrationsPath);
        }
    }
}
