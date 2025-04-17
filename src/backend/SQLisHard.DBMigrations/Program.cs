using CommandLine;
using Microsoft.Data.SqlClient;
using SQLisHard.DBMigrations.Utilities;
using SQLisHard.DBMigrations.Utilities.Logging;

namespace SQLisHard.DBMigrations
{
    public class Options
    {
        [Option('c', "connectionString", Required = true, HelpText = "Database connection string to upgrade")]
        public string ConnectionString { get; set; }

        [Option('m', "migrations", Required = true, HelpText = "Path to appropriate folder of migrations")]
        public string MigrationsFolder { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            int returnValue = 0;

            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(o =>
                {
                    Console.WriteLine($"{o} is not a recognized option");
                    returnValue = -1;
                })
                .WithParsed(o =>
                {
                    try
                    {
                        PerformDatabaseMigration(o.ConnectionString, o.MigrationsFolder);
                    }
                    catch (MigrationFailureException)
                    {
                        returnValue = -1;
                    }
                    catch (Exception)
                    {
                        returnValue = -1;
                    }
                    finally
                    {
#if DEBUG
                        // if we're running this from command line to test it, stop so we can see output
                        Console.WriteLine("(Press enter to continue)");
                        Console.ReadLine();
#endif
                    }
                });
            return returnValue;
        }

        private static void PerformDatabaseMigration(string connString, string migrationsPath)
        {
            List<LogEntry> log;
            try
            {
                // get db info for output purposes
                var sqlConn = new SqlConnection(connString);
                var databaseName = sqlConn.Database;

                // perform the upgrade
                log = DatabaseMigrator.UpgradeWithLog(connString, migrationsPath);

                Console.WriteLine($"=== Log for {databaseName} ===");
                if (log != null)
                {
                    foreach (var line in log)
                    {
                        if (line.EntryType == LogType.Critical)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Critical] {line.Message}");
                            Console.ResetColor();
                        }
                        else if (line.EntryType == LogType.Error)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Error] {line.Message}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"{line.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"No migration log for this tenant");
                }
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Exception] {exc.ToString()}");
                Console.ResetColor();
                if (exc is MigrationFailureException)
                    throw;
                else
                    throw new MigrationFailureException();
            }

            if (log != null && log.Any(line => line.EntryType == LogType.Critical || line.EntryType == LogType.Error))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Upgrade Failed");
                Console.ResetColor();
                throw new MigrationFailureException();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Success!");
                Console.ResetColor();
            }
        }
    }
}