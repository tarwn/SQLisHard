namespace SQLisHard.DBMigrations.Utilities.Logging
{
    public class LogEntry
    {

        public LogEntry(LogType entryType, string message)
        {
            EntryType = entryType;
            Message = message;
        }

        public LogType EntryType { get; }

        public string Message { get; }
    }
}
