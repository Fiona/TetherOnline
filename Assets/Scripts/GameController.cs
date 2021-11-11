using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        test_postgres();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void test_postgres()
    {
        var connString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=tetheronline";

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

// Insert some data
/*
        await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
        {
            cmd.Parameters.AddWithValue("p", "Hello world");
            await cmd.ExecuteNonQueryAsync();
        }
*/
// Retrieve all rows
        await using (var cmd = new NpgsqlCommand("SELECT id,username,superadmin FROM clients", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while(await reader.ReadAsync())
            {
                Debug.Log(reader.GetInt32(0));
                Debug.Log(reader.GetString(1));
                Debug.Log(reader.GetBoolean(2));
            }
        }

    }
}