using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public PlayerContext player;

    private void Update() {
        if(player == null) return;

        Vector3 viewDir = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        player.orientation.forward = viewDir.normalized;
    }
}
