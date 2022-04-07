using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using TetherOnline.Database;
using TetherOnline.Network;
using TetherOnline.NetworkData;
using UnityEngine;


namespace TetherOnline
{

    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [SerializeField]
        public DbSettings databaseSettings;
        [SerializeField]
        public NetworkSettings networkSettings;
        [HideInInspector]
        public Db database;
        [HideInInspector]
        public GlobalNetworkManager networkManager;
        [HideInInspector]
        public ServerInfo serverInfo;

        private CustomLogHandler customLogHandler;

        async void Start()
        {
            Instance = this;

            customLogHandler = new CustomLogHandler();
            var networkManagerObj = new GameObject("Network Manager");
            networkManager = networkManagerObj.AddComponent<GlobalNetworkManager>();
#if IS_SERVER
            await PrepareServer();
#endif
        }


        void Update()
        {

        }

        async Task PrepareServer()
        {
#if IS_SERVER
            Db.settings = databaseSettings;
            var dbObject = new GameObject("Postgresql Database");
            database = dbObject.AddComponent<Db>();

            Log.Message("Preparing server...", "GLOBAL");
            await database.WaitTillReady();
            networkManager.StartServer();
            while(!NetworkServer.active)
                await Task.Delay(15);

            var serverInfoObj = Instantiate(
                NetworkPrefabs.GetNetworkPrefab("ServerInfo"), Vector3.zero, Quaternion.identity
            );
            NetworkServer.Spawn(serverInfoObj);
            serverInfo = serverInfoObj.GetComponent<ServerInfo>();
            serverInfo.serverVersion = Consts.serverVersion;
            serverInfo.serverState = ServerState.Stopped;
            serverInfo.name = "Server Info";
            Log.Message("Server prepared and listening for clients.", "GLOBAL");
#endif
        }
    }

}