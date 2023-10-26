using Scripts.PowerUps;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;

public class PowerUpBuff : NetworkBehaviour {
    public PowerUpBehaviour powerUpBehaviour;
    public PlayerRole Role;

    private void OnTriggerEnter(Collider other) {
        PlayerPowerUp player = other.GetComponent<PlayerPowerUp>();
        if (player == null || player.Role != Role) return;

        AssignPowerClientRPC(player.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void AssignPowerClientRPC(NetworkObjectReference playerNetworkObj) {
        playerNetworkObj.TryGet(out NetworkObject player);
        if (player == null) return;
        PowerUpBehaviour playerPowerUp = player.GetComponent<PlayerPowerUp>().currentPowerUp;
        if (playerPowerUp != null) {
            DestroyPowerServerRpc(playerPowerUp.GetComponent<NetworkObject>());     
        }
        player.GetComponent<PlayerPowerUp>().currentPowerUp = powerUpBehaviour;
        this.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerServerRpc(NetworkObjectReference powerUpObject) {
        if (powerUpObject.TryGet(out NetworkObject powerUp)) {
            Destroy(powerUp.gameObject);
            powerUp.Despawn();
        }
    }
}
