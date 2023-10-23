using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyName;
    [SerializeField] private GameObject startLobby;
    Button startButton;
    TextMeshPro userInputLobbyName;
    void Start()
    {
        startButton = startLobby.gameObject.GetComponent<Button>();
        userInputLobbyName = lobbyName.gameObject.GetComponent<TextMeshPro>();
        startButton.onClick.AddListener(StartLobbyActionHandler);
    }
    private void StartLobbyActionHandler()
    {
        
    }
}
