using System.Collections;
using System.Collections.Generic;
using Scripts.PowerUps;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu()]
public class TeleportPowerUpScriptableObject : PowerUpScriptableObject
{
    public Transform portalPrefab;
    private Transform portalInstance;
    public override void ApplyPowerUp(Transform playerTransform, int castNumber)
    {
        switch (castNumber)
        {
            case 1:
                TeleportFirstCast(playerTransform);
                break;
            case 2:
                TeleportSecondCast(playerTransform);
                break;
        }
    }
    public void TeleportFirstCast(Transform playerTransform) {
        //this cast is for casting a portal.
        InsantiateObjectClientRpc(playerTransform.GetComponent<NetworkObject>());
    }

    public void TeleportSecondCast(Transform playerTransform) {
        //this cast is for returning to the portal whenever you want.
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        rb.position = portalInstance.position;
        DestroyObjectClientRpc(portalInstance.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void InsantiateObjectClientRpc(NetworkObjectReference playerTransform) {
        if (playerTransform.TryGet(out NetworkObject playerNetworkObject)) {
            portalInstance = Instantiate(portalPrefab,playerNetworkObject.GetComponent<Transform>().position,Quaternion.identity);
            
        }
        
    }

    [ClientRpc]
    private void DestroyObjectClientRpc(NetworkObjectReference playerTransform) {
        if (playerTransform.TryGet(out NetworkObject playerNetworkObject)) {
            Destroy(playerNetworkObject.GetComponent<GameObject>());
        }
    }
}
