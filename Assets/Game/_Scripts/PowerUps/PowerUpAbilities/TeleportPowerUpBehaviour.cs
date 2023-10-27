using System;
using Scripts.PowerUps;
using Unity.Netcode;
using UnityEngine;

public class TeleportPowerUpBehaviour : PowerUpBehaviour {
    public Transform portalPrefab;
    private Transform portalInstance;

    public void TeleportFirstCast() {
        portalInstance = Instantiate(portalPrefab, Player.transform.position, Quaternion.identity);
    }

    public void TeleportSecondCast() {
        Rigidbody rb = Player.GetComponent<Rigidbody>();
        rb.position = portalInstance.position;

        if (Player.IsLocalPlayer) {
            DestroyPower();
        }
    }

    public override void ApplyPowerUp() {
        CastPower();

        ApplyPowerUpServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    void ApplyPowerUpServerRPC() {
        ApplyPowerUpClientRPC();
    }

    [ClientRpc]
    void ApplyPowerUpClientRPC() {
        if (Player.IsLocalPlayer) return;

        CastPower();
    }

    private void CastPower() {
        UpdateCast();
        currentCast.Invoke();
        Player.UpdateCounter();
    }

    private void UpdateCast() {
        switch (Player.CastNumber) {
            case 1:
                currentCast = TeleportFirstCast;
                break;
            case 2:
                currentCast = TeleportSecondCast;
                break;
        }
    }

    public override void DestroyPower() {
        DestroyPowerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerServerRpc() {
        if (portalInstance != null) {
            DestroyPortalClientRpc();
        }

        Destroy(gameObject);
    }

    [ClientRpc]
    private void DestroyPortalClientRpc() {
        Destroy(portalInstance.gameObject);
    }
}
