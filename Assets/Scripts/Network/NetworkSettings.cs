using UnityEngine;

namespace TetherOnline.Network
{
    [CreateAssetMenu(fileName = "Network", menuName = "Config/Network Settings", order = 1)]
    public class NetworkSettings: ScriptableObject
    {
        [Header("Network settings")]
        [Tooltip("Listening port")]
        public ushort port = 8100;

        [Header("Debug settings")]
        [Tooltip("Print out Monke encryption transport debugging")]
        public bool enabledMonkeDebugging = false;
    }
}