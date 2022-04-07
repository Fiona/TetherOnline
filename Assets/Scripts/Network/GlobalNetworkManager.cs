using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using TetherOnline.NetworkData;


namespace TetherOnline.Network
{
    public class GlobalNetworkManager : NetworkManager
    {
        public static new GlobalNetworkManager singleton { get; private set; }

        private TelepathyTransport communicationTransport;
        private List<Client> clients = new();
        private GameObject clientContainer;

        public override void Start()
        {
            singleton = this;
            autoStartServerBuild = false;
            autoCreatePlayer = false;
            communicationTransport = gameObject.AddComponent<TelepathyTransport>();
            communicationTransport.port = GameController.Instance.networkSettings.port;
            transport = gameObject.AddComponent<Monke>();
            var monke = transport.GetComponent<Monke>();
            monke.CommunicationTransport = communicationTransport;
            monke.showDebugLogs = GameController.Instance.networkSettings.enabledMonkeDebugging;
            Transport.activeTransport = transport;
            NetworkPrefabs.RegisterNetworkPrefabs(this);
            clientContainer = new GameObject("Clients");
#if IS_CLIENT || IS_ADMIN
            StartClient();
#endif
            base.Start();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
#if IS_SERVER
            var newClient = new Client(conn);
            clients.Add(newClient);
            Log.Message($"New client connected (ID:{conn.connectionId} ADDR:{conn.address})", "GLOBAL");
            var clientObject = Instantiate(NetworkPrefabs.GetNetworkPrefab("Client"), clientContainer.transform);
            clientObject.name = $"Client [ID:{conn.connectionId} ADDR:{conn.address}]";
            NetworkServer.AddPlayerForConnection(conn, clientObject);
            NetworkServer.SetClientReady(conn);
            base.OnServerConnect(conn);
#endif
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
#if IS_SERVER
            clients.Remove(GetClientByNetworkConnection(conn));
            Log.Message($"Client disconnected (ID:{conn.connectionId})", "GLOBAL");
            NetworkServer.SetClientNotReady(conn);
            base.OnServerDisconnect(conn);
#endif
        }

        private Client GetClientByNetworkConnection(NetworkConnectionToClient conn)
        {
            return clients.Find(c => c.networkConnection == conn);
        }
    }
}