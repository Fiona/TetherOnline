using Mirror;
using UnityEngine.Events;

namespace TetherOnline.NetworkData
{

    public enum ServerState
    {
        Stopped = 0,
        Starting = 1,
        Running = 2,
        Stopping = 3
    }

    public class ServerInfo: NetworkBehaviour
    {
        [SyncVar]
        public string serverVersion;

        [SyncVar(hook = nameof(OnServerStateChange))]
        public ServerState serverState;

        [SyncVar]
        public int numPlayers;

        public UnityEvent<ServerState> serverStateChangeEvent;

        public void Start()
        {
            serverStateChangeEvent = new UnityEvent<ServerState>();
        }

        public void OnServerStateChange(ServerState oldValue, ServerState newValue)
        {
            serverStateChangeEvent.Invoke(newValue);
        }
    }

}