using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance { get; private set; }


    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_READY = "IsReady";
    public const string KEY_START_GAME = "JoinCode";


    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler OnLeaveLobby;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }

    private float heartbeatTimer;
    private float lobbyPollTimer;
    private Lobby joinedLobby;
    private string playerName;
    private int isReady;


    private void Awake() {
        Instance = this;
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

    public void SignOut()
    {
        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Signed out! " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignOut();
    }

    private async void HandleLobbyHeartbeat() {
        try
        {
            if (IsLobbyHost())
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;

                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void HandleLobbyPolling() {
        try
        {
            if (joinedLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f)
                {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;

                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    if (!IsPlayerInLobby())
                    {
                        Debug.Log("Kicked from Lobby!");
                        joinedLobby = null;
                    }
                }
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public bool IsPlayerInLobby() {
        if (joinedLobby != null && joinedLobby.Players != null) {
            foreach (Player player in joinedLobby.Players) {
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
        joinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        Debug.Log("Created Lobby " + lobby.LobbyCode);
        return lobby.LobbyCode;
    }

    public async Task<string> JoinLobbyByCode(string lobbyCode) {
        Player player = GetPlayer();
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode[..6], new JoinLobbyByCodeOptions {
            Player = player
        });
        joinedLobby = lobby;
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        Debug.Log("Created Lobby " + lobby.LobbyCode);
        return lobby.LobbyCode;
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
                OnLeaveLobby?.Invoke(this, EventArgs.Empty);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void ToggleReady()
    {
        if (joinedLobby == null) return;
        this.isReady = isReady == 0 ? 1 : 0;
        UpdatePlayerOptions options = new UpdatePlayerOptions{
                Data = new Dictionary<string, PlayerDataObject> {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
                { KEY_PLAYER_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady.ToString()) }
            }
        };
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, options);
            joinedLobby = lobby;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
    }
    public bool IsAllPlayersReady()
    {
        if (joinedLobby == null || joinedLobby.Players == null || joinedLobby.Players.Count != 5)
            return false;

        foreach (Player player in joinedLobby.Players)
        {
            if (player.Data["IsReady"].Value == "0")
            {
                return false;
            }
        }
        return true;
    }
    public void StartGame()
    {
        Debug.Log("Attempt to start a game");
        if (IsLobbyHost() && IsAllPlayersReady())
        {
            Debug.Log("Start Game");
            // List of the players: joinedLobby.Players
        }
    }

}