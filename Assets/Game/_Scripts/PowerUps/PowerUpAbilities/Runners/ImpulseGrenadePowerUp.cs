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
        ThrowGrenade();
        ThrowGrenadeServerRpc();

        DestroyPower();
    }

    private void ThrowGrenade() {
        Transform grenade = Instantiate(grenadePrefab, Player.transform.position, Quaternion.identity);
        grenade.GetComponent<GrenadeCollision>().ColliderToIgnore = Player.GetComponent<Collider>();

        grenade.GetComponent<Rigidbody>()
            .AddForce(Player.GetComponent<PlayerContext>().cameraForward.Value * throwingSpeed, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenadeServerRpc() {
        ThrowGrenadeClientRpc();
    }

    [ClientRpc]
    private void ThrowGrenadeClientRpc() {
        if (Player.IsLocalPlayer) return;

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
