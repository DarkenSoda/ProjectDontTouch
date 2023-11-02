using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour {
    public Button leaveButton;

    private void Awake() {
        leaveButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(0);
        });
    }
}
