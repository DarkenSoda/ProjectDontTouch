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
    
    private void Start() {
        playerInputAction.Abilities.ApplyPowerUp.performed += OnApplyPowerUpPerformed;
    }

    private void OnApplyPowerUpPerformed(InputAction.CallbackContext ctx) {
        if (powerUpEquipedScriptableObject != null) {
            powerUpEquipedScriptableObject.ApplyPowerUp(this.transform);
            powerUpEquipedScriptableObject = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        PowerUpBuff powerUpBuff = other.gameObject.GetComponent<PowerUpBuff>();
        if (powerUpBuff != null) {
            powerUpEquipedScriptableObject = powerUpBuff.GetPowerUpScriptableObject();
            other.GetComponent<NetworkObject>().Despawn(); //switch to despawn on network.
        }
    }
}
