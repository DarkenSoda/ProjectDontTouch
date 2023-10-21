using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkObjectManager : MonoBehaviour
{
    public NetworkObjectManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public static void SpawningObjectServerRpc(Transform Object) {
        Object.GetComponent<NetworkObject>().Spawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public static void DespawningObjectServerRpc(Transform Object) {
        Object.GetComponent<NetworkObject>().Despawn();
    }
}
