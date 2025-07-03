using Microsoft.Data.SqlClient;
using SQLisHard.DBMigrations.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.DBMigrations
{
    public class LocalDatabaseMigrator
    {
        public static void Execute(string connectionString, string migrationsFolderPath)
        {
            var sqlConn = new SqlConnection(connectionString);
            var databaseName = sqlConn.Database;

            Console.WriteLine($"UPG: === Starting Upgrade for: {databaseName} ===");
            Console.WriteLine($"UPG: Migrations folder: {migrationsFolderPath}");
            try
            {

                var log = DatabaseMigrator.UpgradeWithLog(connectionString, migrationsFolderPath);
                foreach (var entry in log)
                {
                    switch (entry.EntryType)
                    {
                        case LogType.Info:
                        case LogType.Warning:
                            Console.WriteLine($"UPG: {entry.EntryType.ToString()} - {entry.Message}");
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine($"UPG: {entry.EntryType.ToString()} - {entry.Message}");
                            Console.ResetColor();
                            break;
                    }
                }

                if (log.Any(e => e.EntryType == LogType.Error || e.EntryType == LogType.Critical))
                {
                    throw new Exception($"Upgrade failed for {databaseName}, see log for details.");
                }
            }
            finally
            {
                Console.WriteLine($"UPG: === Done Upgrade for: {databaseName} ===");
            }
        }
    }
}