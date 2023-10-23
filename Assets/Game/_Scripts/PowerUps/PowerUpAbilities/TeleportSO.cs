using Scripts.PowerUps;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Teleport")]
public class TeleportSO : PowerUpScriptableObject {
    public Transform portalPrefab;
    private Transform portalInstance;
   
    public override void ApplyPowerUp(Transform playerTransform, int castNumber) {
        switch (castNumber) {
            case 1:
                TeleportFirstCast(playerTransform);
                break;
            case 2:
                TeleportSecondCast(playerTransform);
                break;
        }
    }

    public void TeleportFirstCast(Transform playerTransform) {
        portalInstance = Instantiate(portalPrefab, playerTransform.position, Quaternion.identity);
    }

    public void TeleportSecondCast(Transform playerTransform) {
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        rb.position = portalInstance.position;
        Destroy(portalInstance.gameObject);
    }
}
