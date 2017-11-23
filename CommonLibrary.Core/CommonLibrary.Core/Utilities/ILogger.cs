using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utilities
{
    public interface ILogger
    {
        void Debug(string logMessage);

        void Info(string logMessage);

        void Warn(string logMessage);

        void Warn(Exception exc);

        void Error(string logMessage);

        void Error(Exception exc);

        void Fatal(string logMessage);

        void Fatal(Exception ex);
    }
}
