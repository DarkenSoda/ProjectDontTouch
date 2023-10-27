using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LobbyManager;

public class LobbyMenu : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TextMeshProUGUI lobbyNameInput;
    [SerializeField] private TextMeshProUGUI lobbyCodeInput;
    [SerializeField] private TextMeshProUGUI displayNameInput;

    [Header("Button Fields")]
    [SerializeField] private Button startLobby;
    [SerializeField] private Button joinLobby;

    [Header("Lobby UI Fields")]
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    void Start()
    {
        lobbyUI.SetActive(false);
        lobbyCanvas.SetActive(true);
        startLobby.onClick.AddListener(StartLobbyClickHandler);
        joinLobby.onClick.AddListener(JoinLobbyClickHandler);
        LobbyManager.Instance.OnLeaveLobby += Player_OnLeaveLobby;
        LobbyManager.Instance.OnGameStart += StartGame;
    }

    private void Player_OnLeaveLobby(object sender, System.EventArgs e)
    {
        LobbyManager.Instance.SignOut();
        lobbyUI.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    async void StartLobbyClickHandler(){
        try
        {
            await LobbyManager.Instance.Authenticate(displayNameInput.text.ToString()[..(displayNameInput.text.Length - 1)]);
            string lobbyCode = await LobbyManager.Instance.CreateLobby(lobbyNameInput.text);
            if(!string.IsNullOrEmpty(lobbyCode))
            {
                lobbyUI.SetActive(true);
                lobbyCanvas.SetActive(false);
                this.lobbyCode.text = lobbyCode;
            }
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    async void JoinLobbyClickHandler()
    {
        try
        {
            await LobbyManager.Instance.Authenticate(displayNameInput.text.ToString()[..(displayNameInput.text.Length - 1)]);
            string lobbyCode = await LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text.ToString());
            if(!string.IsNullOrEmpty(lobbyCode))
            {
                lobbyUI.SetActive(true);
                lobbyCanvas.SetActive(false);
                this.lobbyCode.text = lobbyCode;
            }
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void StartGame(object sender, EventArgs e) {
        SceneManager.LoadScene(1);
    }
}
