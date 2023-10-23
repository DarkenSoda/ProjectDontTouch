using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyPlayerCard[] playerCards;
    void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += Player_OnJoinedLobby;
        Player_OnJoinedLobby(null, null);
    }

    private void Player_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();
        for (int i = 0; i < lobby.Players.ToArray().Length; i++)
        {
            playerCards[i].SetPlayer(lobby.Players[i]);
        }
    }
}
