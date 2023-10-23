using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button startLobby;
    void Start()
    {
        startLobby.onClick.AddListener(StartLobbyActionHandler);
        LobbyManager.Instance.Authenticate("Player1");
    }
    private void StartLobbyActionHandler()
    {
        LobbyManager.Instance.CreateLobby(lobbyName.text);
    }
}
