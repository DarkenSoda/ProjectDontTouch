using System.Collections;
using System.Collections.Generic;
using Scripts.PowerUps;
using Unity.Netcode;
using UnityEngine;

public class ImpulseGrenadePowerUp : PowerUpBehaviour {
    [SerializeField] private Transform grenadePrefab;

    [Header("Grenade Speed")]
    [SerializeField] private float throwingSpeed;

    public override void ApplyPowerUp() {
        ThrowGrenadeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenadeServerRpc() {
        Transform grenade = Instantiate(grenadePrefab, Player.transform.position, Quaternion.identity);
        grenade.GetComponent<NetworkObject>().Spawn();
        grenade.GetComponent<GrenadeCollision>().ColliderToIgnore = Player.GetComponent<Collider>();

        // Camera is null for clients
        grenade.GetComponent<Rigidbody>()
            .AddForce(Player.GetComponent<PlayerContext>().PlayerCamera.transform.forward * throwingSpeed, ForceMode.Impulse);

        DestroyPower();
    }

    public override void DestroyPower() {
        Destroy(gameObject);
    }
}
