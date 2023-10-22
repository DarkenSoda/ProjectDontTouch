using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.PowerUps;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerPowerUpSystem : NetworkBehaviour
{
    private PowerUpScriptableObject powerUpEquipedScriptableObject;
    private PlayerInputAction playerInputAction;
    private int castCounter;

    public enum PlayerPowerUp {
        Tagger,
        Runner,
    }
    private PlayerPowerUp playerRole;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {
            this.enabled = false;
            return;
        }

        castCounter = 1;
        playerInputAction = new();
        playerInputAction.Abilities.Enable();
        playerInputAction.Abilities.ApplyPowerUp.performed += OnApplyPowerUpPerformed;
    }

    private void OnApplyPowerUpPerformed(InputAction.CallbackContext ctx) {
        if (powerUpEquipedScriptableObject != null) {
            powerUpEquipedScriptableObject.ApplyPowerUp(this.transform, castCounter);
            if (castCounter == powerUpEquipedScriptableObject.numberOfCasts) {
                castCounter = 1;
                powerUpEquipedScriptableObject = null;
                return;
            }
            castCounter++;
        }
    }

    private void OnTriggerEnter(Collider other) {
        PowerUpBuff powerUpBuff;
        if (other.gameObject.TryGetComponent<PowerUpBuff>(out powerUpBuff)) {
            if (powerUpBuff.GetPowerUpScriptableObject().powerUpState == playerRole) {
                powerUpEquipedScriptableObject = powerUpBuff.GetPowerUpScriptableObject();
                NetworkObjectManager.DestroyObjectClientRpc(other.transform);
            }
        }
    }

    public PlayerPowerUp GetPlayerPowerUpState() {
        return playerRole;
    }
    public void SetPlayerPowerUpState(PlayerPowerUp playerRole) {
        this.playerRole = playerRole;
    }
}
