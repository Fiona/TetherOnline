#if IS_SERVER
using Npgsql.Logging;
#endif
using UnityEngine;

namespace TetherOnline.Database
{
    [CreateAssetMenu(fileName = "Db", menuName = "Config/Db Settings", order = 1)]
    public class DbSettings: ScriptableObject
    {
#if IS_SERVER
        [Header("Postgresql server connection")]
        [Tooltip("Hostname")]
        public string hostName = "127.0.0.1";

        [Tooltip("Postgres port")]
        public int port = 5432;

        [Tooltip("Postgres user")]
        public string user = "postgres";

        [Tooltip("Postgres password")]
        public string password;

        [Tooltip("Postgres database name")]
        public string database;

        [Header("Runtime settings")]
        [Tooltip("Seconds until connection is given up on")]
        public int connectionTimeout = 15;

        [Tooltip("Level of debug logs from Postgresql to output to the console")]
        public NpgsqlLogLevel consoleLogLevel = NpgsqlLogLevel.Error;
#endif
    }
}