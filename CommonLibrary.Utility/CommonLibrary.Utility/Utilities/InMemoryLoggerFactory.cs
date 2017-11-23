using CommonLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utilities
{
    public class InMemoryLoggerFactory : ILoggerFactory
    {
        public ILogger CreateDefaultLogger()
        {
            return new InMemoryLogger();
        }

        public ILogger CreateLogger(string loggerName)
        {
            return new InMemoryLogger(loggerName);
        }

        public ILogger CreateLogger(Type type)
        {
            return new InMemoryLogger(type);
        }
    }
}
