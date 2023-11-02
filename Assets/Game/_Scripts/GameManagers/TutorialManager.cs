using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : NetworkBehaviour {
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private Transform hostSpawnPoint;
    [SerializeField] private Transform dummyPlayerSpawnPoint;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private float spawnCooldown;

    private Transform spawnedPlayer;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

    }

    private void Start() {
        NetworkManager.Singleton.StartHost();

        if (IsServer) {
            NetworkManager.ConnectedClients[NetworkManager.ServerClientId].PlayerObject
                                .GetComponent<Rigidbody>().position = hostSpawnPoint.position;
            DummyPlayerServerRpc();
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerContext>().IsHost) {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().position = hostSpawnPoint.position;
        } else {
            DummyPlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DummyPlayerServerRpc() {
        StartCoroutine(SpawnPlayerCoroutine());
    }

    IEnumerator SpawnPlayerCoroutine() {
        if (spawnedPlayer != null) {
            Destroy(spawnedPlayer.gameObject);
        }

        yield return new WaitForSeconds(spawnCooldown);

        spawnedPlayer = Instantiate(playerPrefab, dummyPlayerSpawnPoint.position, Quaternion.identity);
        spawnedPlayer.GetComponent<Swing>().enabled = false;
        spawnedPlayer.GetComponent<PlayerContext>().enabled = false;
        // spawnedPlayer.GetComponent<NetworkObject>().Spawn();
        spawnedPlayer.GetComponent<PlayerPowerUp>().Role.Value = PlayerRole.Runner;
    }
}

