using System;
using System.IO;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace TetherOnline.NetworkData
{
    public static class NetworkPrefabs
    {
        private static readonly string resourcesDir = "Network";
        private static readonly Dictionary<string, string> prefabPaths = new (){
            {"ServerInfo", "ServerInfo"},
            {"Client", "Client"}
        };
        private static Dictionary<string, GameObject> prefabObjects = new();

        public static GameObject GetNetworkPrefab(string prefabName)
        {
            return prefabObjects[prefabName];
        }

        public static Guid GetAssetId(string prefabName)
        {
            return prefabObjects[prefabName].GetComponent<NetworkIdentity>().assetId;
        }

        private static string GetPrefabResourcePath(string prefabName)
        {
            return Path.Join(
                "Prefabs", resourcesDir, prefabPaths[prefabName]
            );
        }

        public static void RegisterNetworkPrefabs(NetworkManager networkManager)
        {
            foreach(var prefabKV in prefabPaths)
            {
                var prefab = Resources.Load(GetPrefabResourcePath(prefabKV.Key)) as GameObject;
                networkManager.spawnPrefabs.Add(prefab);
                prefabObjects[prefabKV.Key] = prefab;
            }
        }

    }
}