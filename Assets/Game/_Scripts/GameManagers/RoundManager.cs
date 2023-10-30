using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class RoundManager : NetworkBehaviour {
    public static RoundManager Instance { get; private set; }

    private int currentRound = 1;
    private float roundTimer;
    private int taggersCount;
    private int runnersCount;
    private bool isTaggerSelected;
    private bool isRoundStarted;

    [SerializeField] private float maxRoundTime;
    [SerializeField] private List<Transform> taggersSpawnPoint;
    [SerializeField] private List<Transform> runnerssSpawnPoint;
    [SerializeField] private Transform playerPrefab;

    public Action RoundStartAction;
    public Action RoundEndAction;
    public Action GameEndAction;

    private void Awake() {
        if (!IsServer) enabled = false;

        if (Instance != null & Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        if (isRoundStarted) {
            roundTimer -= Time.deltaTime;
            CheckRoundEnd();
        }
    }

    public void StartRound() {
        if (currentRound > NetworkManager.ConnectedClients.Count) {
            GameManager.Instance.EndGame();
            return;
        }

        taggersCount = runnersCount = 0;
        foreach (var player in NetworkManager.ConnectedClientsIds) {
            Transform playerObj;
            if (!isTaggerSelected && !GameManager.Instance.Players[player].Value.BeenTaggerBefore) {
                GameManager.Instance.Players[player].Value.BeenTaggerBefore = true;
                isTaggerSelected = true;
                playerObj = Instantiate(playerPrefab, taggersSpawnPoint[taggersCount++].position, Quaternion.identity);
                playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(player, true);

                playerObj.GetComponent<PlayerPowerUp>().Role.Value = PlayerRole.Tagger;
            } else {
                playerObj = Instantiate(playerPrefab, runnerssSpawnPoint[runnersCount++].position, Quaternion.identity);
                playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(player, true);

                playerObj.GetComponent<PlayerPowerUp>().Role.Value = PlayerRole.Runner;
            }
        }

        // 3 2 1
        isRoundStarted = true;
        roundTimer = maxRoundTime;
        // Start round / Allow movement / countdown Timer
    }

    public void CheckRoundEnd() {
        if (taggersCount == 0 || runnersCount == 0 || roundTimer <= 0) {
            EndRound();
        }
    }

    public void EndRound() {
        isRoundStarted = false;
        DespawnPlayers();
        // Show Round winner (Tagger/Runners)

        if (runnersCount == 0) {
            // Tagger win
        } else {
            // Runners win
        }

        // score
        // 3 2 1

        isTaggerSelected = false;
        currentRound++;
        StartRound();
    }

    private void DespawnPlayers() {
        foreach (var player in NetworkManager.ConnectedClientsList) {
            player.PlayerObject.Despawn();
        }
    }
}
