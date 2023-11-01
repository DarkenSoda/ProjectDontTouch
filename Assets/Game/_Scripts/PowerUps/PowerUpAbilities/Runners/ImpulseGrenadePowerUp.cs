using System.Collections;
using System.Collections.Generic;
using Scripts.PowerUps;
using Unity.Netcode;
using UnityEngine;

public class ImpulseGrenadePowerUp : PowerUpBehaviour {
    [SerializeField] private Transform grenadePrefab;

    [Header("Grenade Speed")]
    [SerializeField] private float throwingSpeed;

    private Transform grenade;

    public override void ApplyPowerUp() {
        ThrowGrenadeServerRpc();

        DestroyPower();
    }

    private void ThrowGrenade() {
        grenade.GetComponent<Rigidbody>()
            .AddForce(Player.GetComponent<PlayerContext>().cameraForward.Value * throwingSpeed, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenadeServerRpc() {
        grenade = Instantiate(grenadePrefab, Player.transform.position, Quaternion.identity);
        grenade.GetComponent<NetworkObject>().Spawn();
        grenade.GetComponent<GrenadeCollision>().ColliderToIgnore = Player.GetComponent<Collider>();

        ThrowGrenade();
    }

    public override void DestroyPower() {
        DestroyPowerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerServerRpc() {
        Destroy(gameObject);
    }
}
