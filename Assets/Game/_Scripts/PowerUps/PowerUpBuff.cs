using Scripts.PowerUps;
using UnityEngine;
using Unity.Netcode;

public class PowerUpBuff : NetworkBehaviour {
    public PowerUpScriptableObject PowerUpScriptableObject;
    public PlayerRole Role;

    private void OnTriggerEnter(Collider other) {
        PlayerPowerUp player = other.GetComponent<PlayerPowerUp>();
        if (player == null || player.Role != Role) return;

        player.currentPowerUp = PowerUpScriptableObject;
        DespawnPowerServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnPowerServerRPC() {
        Destroy(gameObject);
        GetComponent<NetworkObject>().Despawn();
    }
}
