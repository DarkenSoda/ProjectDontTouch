using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class GameManager : NetworkBehaviour {
    public class PlayerData : INetworkSerializable {
        public float Score;
        public bool BeenTaggerBefore;
        public bool IsAlive;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            Score.Serialize();
            BeenTaggerBefore.Serialize();
            IsAlive.Serialize();
        }
    }
    public static GameManager Instance { get; private set; }
    public Dictionary<ulong, NetworkVariable<PlayerData>> Players;
    private void Awake() {
        if (!IsServer) enabled = false;

        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        Players = new Dictionary<ulong, NetworkVariable<PlayerData>>();
    }

    private void Start() {
        NetworkManager.OnClientConnectedCallback += OnPlayerConnected;
        NetworkManager.OnClientDisconnectCallback += OnPlayerDisconnected;

        foreach (var player in NetworkManager.ConnectedClientsIds) {
            OnPlayerConnected(player);
        }

        RoundManager.Instance.StartRound();
    }

    public void EndGame() {
        Debug.Log("Game Over");
        // Show Scoreboard
        // Return to lobby
    }

    private void OnPlayerConnected(ulong clientId) {
        Players.Add(clientId, new NetworkVariable<PlayerData>(new PlayerData { Score = 0, BeenTaggerBefore = false, IsAlive = false }));
    }

    private void OnPlayerDisconnected(ulong clientId) {
        RoundManager.Instance.KillPlayer(clientId);
        Players.Remove(clientId);
    }
}