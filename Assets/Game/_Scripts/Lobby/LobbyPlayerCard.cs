using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TextMeshProUGUI playerDisplayName;
    public void SetPlayer(Player player)
    {
        playerPanel.SetActive(true);
        playerDisplayName.text = player.Data["PlayerName"].Value;
    }
}