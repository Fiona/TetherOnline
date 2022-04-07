using Mirror;


namespace TetherOnline.Network
{
    public class Client
    {
        public NetworkConnectionToClient networkConnection;

        public Client(NetworkConnectionToClient networkConnection)
        {
            this.networkConnection = networkConnection;
        }
    }
}