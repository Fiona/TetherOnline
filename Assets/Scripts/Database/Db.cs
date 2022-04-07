using System.Data;
#if IS_SERVER
using Npgsql;
using Npgsql.Logging;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace TetherOnline.Database
{
    public class Db: MonoBehaviour
    {
#if IS_SERVER
        public static DbSettings settings;
        public bool ready;

        private string connectionString => $"Host={settings.hostName};Port={settings.port};Username={settings.user};Password={settings.password};Database={settings.database};Timeout={settings.connectionTimeout}";

        public async void Start()
        {
            Log.Message("PostgreSQL database object using connection settings:", "DB");
            Log.Message(connectionString, "DB");
            NpgsqlLogManager.Provider = new DbUnityConsoleLoggerProvider(settings.consoleLogLevel);
            var success = await TestConnection();
            ready = success;
            if(!ready)
                Log.Error("There was an error testing connection to the database.", "DB");
            //await ExampleSelect();
        }

        public async Task WaitTillReady()
        {
            while(!ready)
                await Task.Delay(15);
        }

        async Task<NpgsqlConnection> CreateConnection()
        {
            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }

        async Task<bool> TestConnection()
        {
            var connection = await CreateConnection();
            Log.Message("Testing database connection...", "DB");
            var connected = (connection.State & ConnectionState.Open) != 0;
            await connection.CloseAsync();
            if(connected)
                Log.Success("Success", "DB");
            else
                Log.Error("Failed", "DB");
            return connected;
        }

        async void ExampleInsert()
        {
            // Insert some data
            /*
            await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", "Hello world");
                await cmd.ExecuteNonQueryAsync();
            }
            */

        }

        async Task ExampleSelect()
        {
            await WaitTillReady();

            var connection = await CreateConnection();

            // Retrieve all rows
            await using(var cmd = new NpgsqlCommand("SELECT id,username,superadmin FROM clients", connection))
            await using(var reader = await cmd.ExecuteReaderAsync())
            {
                while(await reader.ReadAsync())
                {
                    Debug.Log(reader.GetInt32(0));
                    Debug.Log(reader.GetString(1));
                    Debug.Log(reader.GetBoolean(2));
                }
            }

            await connection.CloseAsync();
        }
#endif
    }
}