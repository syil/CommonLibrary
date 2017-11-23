using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utilities
{
    public interface ILoggerFactory
    {
        ILogger CreateDefaultLogger();
        ILogger CreateLogger(string loggerName);
        ILogger CreateLogger(Type type);
    }
}
