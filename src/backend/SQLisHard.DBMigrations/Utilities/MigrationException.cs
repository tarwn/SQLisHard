using DbUp.Engine;

namespace SQLisHard.DBMigrations.Utilities
{
    public class MigrationException : Exception
    {
        public MigrationException(string message, DatabaseUpgradeResult result)
            : base(message)
        {
            Result = result;
        }

        public DatabaseUpgradeResult Result { get; }
    }
}
