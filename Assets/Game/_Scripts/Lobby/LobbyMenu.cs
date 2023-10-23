using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TextMeshProUGUI lobbyNameInput;
    [SerializeField] private TextMeshProUGUI lobbyCodeInput;

    [Header("Button Fields")]
    [SerializeField] private Button startLobby;
    [SerializeField] private Button joinLobby;

    [Header("Lobby UI Fields")]
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    void Start()
    {
        lobbyCanvas.SetActive(true);
        lobbyMenu.SetActive(false);
        LobbyManager.Instance.Authenticate("Player1");
        startLobby.onClick.AddListener(StartLobbyClickHandler);
        joinLobby.onClick.AddListener(JoinLobbyClickHandler);
    }
    async void StartLobbyClickHandler(){
        string lobbyCode = await LobbyManager.Instance.CreateLobby(lobbyNameInput.text);
        if(!string.IsNullOrEmpty(lobbyCode))
        {
            lobbyMenu.SetActive(true);
            this.lobbyCode.text = lobbyCode;
            lobbyCanvas.SetActive(false);
        }
    }
    void JoinLobbyClickHandler()
    {
        LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
    }
}
