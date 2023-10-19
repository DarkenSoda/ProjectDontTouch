using Cinemachine;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public PlayerContext player;
    public CinemachineFreeLook freeLookCamera;

    private void Update() {
        if(player == null) return;

        Vector3 viewDir = transform.forward;
        viewDir.y = 0;
        player.orientation.forward = viewDir.normalized;
    }
}
