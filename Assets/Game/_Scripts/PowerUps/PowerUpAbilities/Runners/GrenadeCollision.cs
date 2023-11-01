using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrenadeCollision : NetworkBehaviour {
    public Collider ColliderToIgnore { get; set; }

    [SerializeField] private float impulseForce;
    [SerializeField] private float collisionRadius;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("A");
        if (other != ColliderToIgnore) {
            Debug.Log("B");
            if (ColliderToIgnore.GetComponent<PlayerPowerUp>().IsLocalPlayer) {
                Debug.Log("C");
                ApplyImpulseServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyImpulseServerRpc() {
        Debug.Log("D");
        DisableObjectClientRpc();                       // Disable Grenade visuals

        var playersHit = Physics.OverlapSphere(transform.position, collisionRadius, playerLayer);
        foreach (var player in playersHit) {
            ImpulsePlayerClientRpc(player.GetComponent<NetworkObject>());
        }

        GetComponent<NetworkObject>().Despawn();        // Despawn Grenade
    }

    [ClientRpc]
    private void ImpulsePlayerClientRpc(NetworkObjectReference playerObj) {
        Debug.Log("E");
        if (playerObj.TryGet(out NetworkObject player)) {
            player.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * impulseForce, ForceMode.Impulse);
        }
    }

    [ClientRpc]
    private void DisableObjectClientRpc() {
        gameObject.SetActive(false);
    }
}
