using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TextMeshProUGUI playerDisplayName;
    [SerializeField] private GameObject readyMark;
    private void Start()
    {
        readyMark.SetActive(false);
    }
    public void SetPlayer(Player player)
    {
        playerPanel.SetActive(true);
        playerDisplayName.text = player.Data["PlayerName"].Value;
        int isReady = int.Parse(player.Data["IsReady"].Value);
        Debug.Log(isReady);
        if (isReady == 0)
        {
            readyMark.SetActive(false);
        }
        else
        {
            readyMark.SetActive(true);
        }
    }
    public void RemovePlayer()
    {
        playerPanel.SetActive(false);
        playerDisplayName.text = "Display Name";
    }
}