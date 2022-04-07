using System;
using UnityEngine;
using TetherOnline;

namespace TetherOnline.Database
{
#if IS_SERVER
    using Npgsql.Logging;

    class LogLoggerProvider : INpgsqlLoggingProvider
    {
        private NpgsqlLogLevel minLevel;

        public LogLoggerProvider(NpgsqlLogLevel minLevel)
        {
            this.minLevel = minLevel;
        }

        public NpgsqlLogger CreateLogger(string name)
        {
            return new LogLogger(name, minLevel);
        }
    }

    class LogLogger : NpgsqlLogger
    {
        private NpgsqlLogLevel minLevel;

        internal LogLogger(string name, NpgsqlLogLevel minLevel)
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
            switch(level)
            {
                case NpgsqlLogLevel.Error:
                case NpgsqlLogLevel.Fatal:
                    TetherOnline.Log.Error(msg, "DB");
                    break;
                case NpgsqlLogLevel.Warn:
                    TetherOnline.Log.Warning(msg, "DB");
                    break;
                default:
                    TetherOnline.Log.Message(msg, "DB");
                    break;
            }
            if(exception != null)
                TetherOnline.Log.Error(exception.Message, "DB");
        }
    }
#endif
}