using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class SwingableObjectUI : MonoBehaviour
{
    public Transform swingableObject; 
    public Camera playerCamera;
    public Image targetLockImage;

    private void Start() {
        this.gameObject.SetActive(false);
    }
    private void Update() {
        if (playerCamera != null) {
            targetLockImage.transform.position = playerCamera.WorldToScreenPoint(swingableObject.position);
        }
    }
}
