using System;
using System.Collections.Generic;
using System.Threading;

namespace AsyncContextFlowStudy
{
    public class TestLog
    {
        public static TestLog Instance { get; } = new TestLog();

        readonly object _lock = new object();
        readonly List<LogEntry> _logs = new List<LogEntry>();

        private TestLog() { }

        public void Write(Guid correlationId, string entry)
        {
            lock (_lock)
            {
                var identity = Thread.CurrentPrincipal.Identity;
                _logs.Add(new LogEntry
                {
                    CorrelationId = correlationId,
                    Message = entry,
                    Principal = string.IsNullOrEmpty(identity?.Name) ? null : identity.Name,
                    ThreadId = Convert.ToString(Thread.CurrentThread.ManagedThreadId),
                    Timestamp = DateTime.UtcNow,
                });
            }
        }

        public IEnumerable<LogEntry> GetEntries()
        {
            lock (_lock)
            {
                return _logs.ToArray();
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }
    }
    
    public class LogEntry
    {
        public Guid CorrelationId { get; set; }
        public string FormattedId => CorrelationId.ToString().Substring(0, 6);

        public string Message { get; set; }

        public string Principal { get; set; }
        public string FormattedPrincipal => Principal ?? "<anonymous>"; public string ThreadId { get; set; }

        public DateTime Timestamp { get; set; }
        public string FormattedTimestamp => $"{Timestamp:HH:mm:ss.fff}";
    }
}