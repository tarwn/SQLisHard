using DbUp.Engine.Output;
using SQLisHard.DBMigrations.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.DBMigrations.Utilities
{
    public class StringLogger : IUpgradeLog
    {
        private Queue<LogEntry> _log;

        public StringLogger()
        {
            _log = new Queue<LogEntry>();
        }

        public void WriteError(string format, params object[] args)
        {
            _log.Enqueue(new LogEntry(LogType.Error, String.Format(format, args)));
        }

        public void WriteInformation(string format, params object[] args)
        {
            _log.Enqueue(new LogEntry(LogType.Info, String.Format(format, args)));
        }

        public void WriteWarning(string format, params object[] args)
        {
            _log.Enqueue(new LogEntry(LogType.Warning, String.Format(format, args)));
        }

        public List<LogEntry> GetLog()
        {
            return _log.ToList();
        }

        /// <summary>
        /// This is not part of IUpgradeLog, it's used for critical errors during our 
        /// migration process
        /// </summary>
        public void WriteCriticalError(string message)
        {
            _log.Enqueue(new LogEntry(LogType.Critical, message));
        }
    }
}
