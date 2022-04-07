using System;
using UnityEngine;

namespace TetherOnline.Database
{
#if IS_SERVER
    using Npgsql.Logging;

    class DbUnityConsoleLoggerProvider : INpgsqlLoggingProvider
    {
        private NpgsqlLogLevel minLevel;

        public DbUnityConsoleLoggerProvider(NpgsqlLogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public NpgsqlLogger CreateLogger(string name)
        {
            return new DbUnityConsoleLogger(name, minLevel);
        }
    }

    class DbUnityConsoleLogger : NpgsqlLogger
    {
        private NpgsqlLogLevel minLevel;

        internal DbUnityConsoleLogger(string name, NpgsqlLogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return level >= minLevel;
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            if(!IsEnabled(level))
                return;
            Debug.Log($"DB[{level}]: {msg}");
            if(exception != null)
                Debug.Log(exception.Message);
        }
    }
#endif
}