using Scripts.PowerUps;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;

public class PowerUpBuff : NetworkBehaviour {
    public PowerUpBehaviour powerUpBehaviour;
    public Transform currentSpawnPoint;
    public PlayerRole Role;
    public Action ClearSpawnPointAction;

    public override void OnNetworkSpawn() {
        if (!IsServer) enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        List<Transform> occupiedSpawnPoints;
        List<Transform> spawnPoints;
        PlayerPowerUp player = other.GetComponent<PlayerPowerUp>();
        if (player == null || player.Role != Role) return;

        if (Role ==  PlayerRole.Tagger) {
            occupiedSpawnPoints = PowerUpManager.Instance.occupiedTaggersSpawnPoints;
            spawnPoints = PowerUpManager.Instance.TaggerSpawnPoints;
        }
        else {
            occupiedSpawnPoints = PowerUpManager.Instance.occupiedRunnersSpawnPoints;
            spawnPoints = PowerUpManager.Instance.RunnerSpawnPoints;
        }

        spawnPoints.Add(currentSpawnPoint);
        occupiedSpawnPoints.Remove(currentSpawnPoint);
        
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
