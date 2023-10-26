using Scripts.PowerUps;
using UnityEngine;
using Unity.Netcode;

public class PowerUpBuff : NetworkBehaviour {
    public PowerUpScriptableObject PowerUpScriptableObject;
    public PlayerRole Role;

    private void OnTriggerEnter(Collider other) {
        PlayerPowerUp player = other.GetComponent<PlayerPowerUp>();
        if (player == null || player.Role != Role) return;

        AssignPowerClientRPC(player.GetComponent<NetworkObject>());
    }

    [ClientRpc]
    private void AssignPowerClientRPC(NetworkObjectReference playerNetworkObj) {
        playerNetworkObj.TryGet(out NetworkObject player);
        if (player == null) return;

        player.GetComponent<PlayerPowerUp>().currentPowerUp = PowerUpScriptableObject;
        DestroyPowerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyPowerServerRpc() {
        Destroy(gameObject);
        GetComponent<NetworkObject>().Despawn();
    }
}
