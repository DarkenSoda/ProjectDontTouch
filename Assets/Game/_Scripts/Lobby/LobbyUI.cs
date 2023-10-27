using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerCard[] playerCards;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject startGameButton;
    void Start()
    {
        LobbyManager.Instance.OnJoinedLobbyUpdate += Player_OnLobbyUpdate;
        leaveButton.onClick.AddListener(LeaveButtonClickHandler);
        readyButton.onClick.AddListener(ReadyButtonClickHandler);
        startGameButton.GetComponent<Button>().onClick.AddListener(StartGameButtonClickHandler);
    }

    private void Player_OnLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = e.lobby;
        for (int i = 0; i < 5; i++)
        {
            if(i < lobby.Players.Count)
                playerCards[i].SetPlayer(lobby.Players[i]);
            else
                playerCards[i].RemovePlayer();
        }
        bool isHost = LobbyManager.Instance.IsLobbyHost();

        if (isHost)
        {
            startGameButton.SetActive(true);
        }
    }
    private void LeaveButtonClickHandler()
    {
        LobbyManager.Instance.LeaveLobby();
    }
    private void ReadyButtonClickHandler()
    {
        LobbyManager.Instance.ToggleReady();
    }
    private void StartGameButtonClickHandler()
    {
        LobbyManager.Instance.StartGame();
    }
}
