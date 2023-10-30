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
        if (IsServer && other != ColliderToIgnore) {
            DisableObjectClientRpc();

            var playersHit = Physics.OverlapSphere(transform.position, collisionRadius, playerLayer);
            foreach (var player in playersHit) {
                // Need to make this ClientRPC
                player.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * impulseForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void DisableObjectClientRpc() {
        gameObject.SetActive(false);
    }
}
