using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Button multiplayerBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitBtn;

    private void Start() {
        tutorialBtn.onClick.AddListener(() => {
            //load tutorial scene and start host.
            SceneManager.LoadScene(1);
        });

        multiplayerBtn.onClick.AddListener(() => {
            //Load multiplayer Lobby.
        });

        creditsBtn.onClick.AddListener(() => {
            //enter the credit scene.
        });

        exitBtn.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
