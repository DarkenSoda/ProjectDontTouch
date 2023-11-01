using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrenadeCollision : NetworkBehaviour {
    public Collider ColliderToIgnore { get; set; }

    [SerializeField] private float impulseForce;
    [SerializeField] private float collisionRadius;
    [SerializeField] private LayerMask playerLayer;

    private NetworkVariable<Vector3> impulsePosition = new NetworkVariable<Vector3>();

    private void OnTriggerEnter(Collider other) {
        if (IsServer && other != ColliderToIgnore) {
            ApplyImpulse();
        }
    }

    private void ApplyImpulse() {
        DisableObjectClientRpc();   // Disable Grenade visuals

        impulsePosition.Value = transform.position;

        var playersHit = Physics.OverlapSphere(transform.position, collisionRadius, playerLayer);
        foreach (var player in playersHit) {
            ImpulsePlayerClientRpc(player.GetComponent<NetworkObject>());
        }

        Destroy(gameObject);        // Despawn Grenade
    }

    [ClientRpc]
    private void ImpulsePlayerClientRpc(NetworkObjectReference playerObj) {
        if (playerObj.TryGet(out NetworkObject player)) {
            player.GetComponent<Rigidbody>().AddForce((player.transform.position - impulsePosition.Value).normalized * impulseForce, ForceMode.Impulse);
        }
    }

    [ClientRpc]
    private void DisableObjectClientRpc() {
        gameObject.SetActive(false);
    }
}
