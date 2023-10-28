using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour {
    public class PlayerData {
        public float Score;
        public bool BeenTaggerBefore;
    }
    public static GameManager Instance { get; private set; }
    public Dictionary<ulong, PlayerData> Players;
    private void Awake() {
        if (!IsServer) enabled = false;

        if (Instance != null & Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        NetworkManager.OnClientConnectedCallback += OnPlayerConnected;
        NetworkManager.OnClientDisconnectCallback += OnPlayerDisconnected;

        foreach (var player in NetworkManager.ConnectedClientsIds) {
            OnPlayerConnected(player);
        }
    }

    public void EndGame() {
        // Show Scoreboard
        // Return to lobby
    }

    private void OnPlayerConnected(ulong clientId) {
        Players.Add(clientId, new PlayerData { Score = 0, BeenTaggerBefore = false });
        RoundManager.Instance.StartRound();
    }

    private void OnPlayerDisconnected(ulong clientId) {
        Players.Remove(clientId);
    }
}