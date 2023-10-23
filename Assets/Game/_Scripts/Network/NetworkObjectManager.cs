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
    [ClientRpc]
    public static Transform InsantiateObjectClientRpc(Transform prefab, Vector3 position) {
        return Instantiate(prefab, position, Quaternion.identity);
    }
    
    [ClientRpc]
    public static void DestroyObjectClientRpc(Transform prefab) {
        Destroy(prefab.gameObject);
    }
}
