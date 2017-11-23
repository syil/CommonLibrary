using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLibrary.Utilities
{
    public class InMemoryLogger : ILogger
    {
        private string loggerName;
        private List<string> inMemoryLogs = new List<string>();

        public InMemoryLogger()
            : this("Default")
        {

        }

        public InMemoryLogger(Type loggerName)
            : this(loggerName.FullName)
        {

        }

        public InMemoryLogger(string loggerName)
        {
            this.inMemoryLogs = new List<string>();
            this.loggerName = loggerName;
        }

        public void Debug(string logMessage)
        {
            inMemoryLogs.Add($"DEBUG: [{logMessage}]");
        }

        public void Info(string logMessage)
        {
            inMemoryLogs.Add($"INFO : [{logMessage}]");
        }

        public void Warn(string logMessage)
        {
            inMemoryLogs.Add($"WARN : [{logMessage}]");
        }

        public void Warn(Exception exc)
        {
            inMemoryLogs.Add($"WARN: [{exc.ToString()}]");
        }

        public void Error(string logMessage)
        {
            inMemoryLogs.Add($"ERROR: [{logMessage}]");
        }

        public void Error(Exception exc)
        {
            inMemoryLogs.Add($"ERROR: [{exc.ToString()}]");
        }

        public void Fatal(string logMessage)
        {
            inMemoryLogs.Add($"FATAL: [{logMessage}]");
        }

        public void Fatal(Exception ex)
        {
            inMemoryLogs.Add($"FATAL: [{ex.ToString()}]");
        }
    }
}
