using Scripts.PowerUps;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;

public class PowerUpBuff : NetworkBehaviour {
    public PowerUpBehaviour powerUpBehaviour;
    public Transform currentSpawnPoint;
    public PlayerRole Role;

    public override void OnNetworkSpawn() {
        if (!IsServer) enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        PlayerPowerUp player = other.GetComponent<PlayerPowerUp>();
        if (player == null || player.Role != Role) return;

        if (player.Role ==  PlayerRole.Tagger) {
            PowerUpManager.Instance.TaggerSpawnPointsAddItem(currentSpawnPoint);
            PowerUpManager.Instance.OccupiedTaggerSpawnPointsRemoveItem(currentSpawnPoint);
        }
        else {
            PowerUpManager.Instance.RunnerSpawnPointsAddItem(currentSpawnPoint);
            PowerUpManager.Instance.OccupiedRunnerSpawnPointsRemoveItem(currentSpawnPoint);
        }
        AssignPowerClientRPC(player.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void AssignPowerClientRPC(NetworkObjectReference playerNetworkObj) {
        playerNetworkObj.TryGet(out NetworkObject player);
        if (player == null) return;
        PlayerPowerUp playerPowerUp = player.GetComponent<PlayerPowerUp>();
        if (playerPowerUp.CurrentPowerUp != null) {
            playerPowerUp.CurrentPowerUp.DestroyPower();
        }
        playerPowerUp.CurrentPowerUp = powerUpBehaviour;
        playerPowerUp.ResetCounter();
        playerPowerUp.SetPowerUpUser();
        this.gameObject.SetActive(false);
    }
}
