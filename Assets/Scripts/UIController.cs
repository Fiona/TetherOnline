using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using StompyBlondie.Utils;
using TetherOnline.NetworkData;


namespace TetherOnline.UI
{

    public enum GlobalUIState
    {
        Disconnected = 0,
        ConnectedAuthenticate = 1,
        AuthenticatedStopped = 2,
        AuthenticatedStarting = 3,
        AuthenticatedRunning = 4
    };

    public class UIController: MonoBehaviour
    {
        public TextMeshPro connectionStatusText;
        public GlobalUIState currentState = GlobalUIState.Disconnected;
        public List<GameObject> globalUIStateContainers;

        private ServerInfo serverInfo;
        private bool authenticated;

        public void Start()
        {
            SwitchState(GlobalUIState.Disconnected);
        }

        public void Update()
        {
            if(serverInfo == null)
            {
                serverInfo = FindObjectOfType<ServerInfo>();
                serverInfo?.serverStateChangeEvent.AddListener(OnServerStateChange);
            }

            if(serverInfo == null && currentState != GlobalUIState.Disconnected)
            {
                authenticated = false;
                SwitchState(GlobalUIState.Disconnected);
            }
        }

        public void SetConnectionStatusText(string newMessage)
        {
            connectionStatusText.text = $"Connection status: {newMessage}";
        }

        public void SwitchState(GlobalUIState newState)
        {
            foreach(var container in globalUIStateContainers)
                container.SetActive(false);
            globalUIStateContainers[(int)newState].SetActive(true);
        }

        private void OnServerStateChange(ServerState newServerState)
        {
            if(!authenticated)
            {
                SwitchState(GlobalUIState.ConnectedAuthenticate);
                return;
            }

            switch(newServerState)
            {
                case ServerState.Stopped:
                    SwitchState(GlobalUIState.AuthenticatedStopped);
                    break;
                case ServerState.Starting:
                    SwitchState(GlobalUIState.AuthenticatedStarting);
                    break;
                case ServerState.Running:
                    SwitchState(GlobalUIState.AuthenticatedRunning);
                    break;
            }
        }
    }
}