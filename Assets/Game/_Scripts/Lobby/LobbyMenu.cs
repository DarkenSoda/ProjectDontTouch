using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
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
    }

    private void Player_OnLeaveLobby(object sender, System.EventArgs e)
    {
        lobbyUI.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    async void StartLobbyClickHandler(){
        await LobbyManager.Instance.Authenticate(displayNameInput.text.ToString()[..(displayNameInput.text.Length - 1)]);
        string lobbyCode = await LobbyManager.Instance.CreateLobby(lobbyNameInput.text);
        if(!string.IsNullOrEmpty(lobbyCode))
        {
            lobbyUI.SetActive(true);
            lobbyCanvas.SetActive(false);
            this.lobbyCode.text = lobbyCode;
        }
    }
    async void JoinLobbyClickHandler()
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
}
