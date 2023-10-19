using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.PowerUps;
using UnityEngine.InputSystem;
public class PlayerPowerUpSystem : MonoBehaviour
{
    private PowerUpScriptableObject powerUpEquipedScriptableObject;
    private PlayerInputAction playerInputAction;
    
    private void Start() {
        playerInputAction.Abilities.ApplyPowerUp.performed += OnApplyPowerUpPerformed;
    }

    private void OnApplyPowerUpPerformed(InputAction.CallbackContext ctx) {
        if (powerUpEquipedScriptableObject != null) {
            powerUpEquipedScriptableObject.ApplyPowerUp();
            powerUpEquipedScriptableObject = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        PowerUpBuff powerUpBuff = other.gameObject.GetComponent<PowerUpBuff>();
        if (powerUpBuff != null) {
            powerUpEquipedScriptableObject = powerUpBuff.GetPowerUpScriptableObject();
            Destroy(other.gameObject); //switch to despawn on network.
        }
    }
}
