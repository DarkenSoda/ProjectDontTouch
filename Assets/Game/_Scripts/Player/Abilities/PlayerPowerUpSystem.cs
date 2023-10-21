using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.PowerUps;
using UnityEngine.InputSystem;
using Unity.Netcode;
public class PlayerPowerUpSystem : MonoBehaviour
{
    private PowerUpScriptableObject powerUpEquipedScriptableObject;
    private PlayerInputAction playerInputAction;
    private int castCounter;
    private void Start() {
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
            }
            castCounter++;
        }
    }

    private void OnTriggerEnter(Collider other) {
        PowerUpBuff powerUpBuff;
        if (other.gameObject.TryGetComponent<PowerUpBuff>(out powerUpBuff)) {
            powerUpEquipedScriptableObject = powerUpBuff.GetPowerUpScriptableObject();
            other.GetComponent<NetworkObject>().Despawn(); //switch to despawn on network.
        }
    }
}
