using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance { get; private set; }


    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY = "IsReady";
    public const string KEY_START_GAME = "JoinCode";

    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler OnLeaveLobby;
    public event EventHandler OnGameStart;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }

    private float heartbeatTimer;
    private float lobbyPollTimer;
    public Lobby JoinedLobby { get; private set; }
    private string playerName;
    private int isReady;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async Task Authenticate(string playerName) {
        this.playerName = playerName;
        this.isReady = 0;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void SignOut() {
        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Signed out! " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignOut();
    }

    private async void HandleLobbyHeartbeat() {
        try {
            if (IsLobbyHost()) {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f) {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;

                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
                }
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private async void HandleLobbyPolling() {
        try {
            if (JoinedLobby != null) {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f) {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;

                    JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
                    if (!IsPlayerInLobby()) {
                        Debug.Log("Kicked from Lobby!");
                        JoinedLobby = null;
                    }
                }
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public bool IsLobbyHost() {
        return JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public bool IsPlayerInLobby() {
        if (JoinedLobby != null && JoinedLobby.Players != null) {
            foreach (Player player in JoinedLobby.Players) {
                if (player.Id == AuthenticationService.Instance.PlayerId) {
                    return true;
                }
            }
        }
        return false;
    }

    private Player GetPlayer() {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_PLAYER_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady.ToString()) }
        });
    }

    public async Task<string> CreateLobby(string lobbyName) {
        Player player = GetPlayer();
        CreateLobbyOptions options = new CreateLobbyOptions {
            Player = player,
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
            {
                {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Public, "0") }
            }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 5, options);
        JoinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
        Debug.Log("Created Lobby " + lobby.LobbyCode);
        return lobby.LobbyCode;
    }

    public async Task<string> JoinLobbyByCode(string lobbyCode) {
        Player player = GetPlayer();
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode[..6], new JoinLobbyByCodeOptions {
            Player = player
        });
        JoinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
        Debug.Log("Created Lobby " + lobby.LobbyCode);
        return lobby.LobbyCode;
    }

    public async void LeaveLobby() {
        if (JoinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
                JoinedLobby = null;
                OnLeaveLobby?.Invoke(this, EventArgs.Empty);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void ToggleReady() {
        if (JoinedLobby == null) return;
        this.isReady = isReady == 0 ? 1 : 0;
        UpdatePlayerOptions options = new UpdatePlayerOptions {
            Data = new Dictionary<string, PlayerDataObject> {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
                { KEY_PLAYER_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady.ToString()) }
            }
        };
        try {
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId, options);
            JoinedLobby = lobby;
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = JoinedLobby });
    }

    public bool IsAllPlayersReady() {
        if (JoinedLobby == null || JoinedLobby.Players == null || JoinedLobby.Players.Count < 2)
            return false;

        foreach (Player player in JoinedLobby.Players) {
            if (player.Data["IsReady"].Value == "0") {
                return false;
            }
        }
        return true;
    }

    public void StartGame() {
        Debug.Log("Attempt to start a game");
        if (IsLobbyHost() && IsAllPlayersReady()) {
            Debug.Log("Start Game");
            // List of the players: joinedLobby.Players
            SceneManager.LoadScene(1);
            // OnGameStart?.Invoke(this, EventArgs.Empty);
        }
    }
}