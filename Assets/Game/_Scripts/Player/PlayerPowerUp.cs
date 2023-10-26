using Scripts.PowerUps;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerUp : NetworkBehaviour {
    [SerializeField] private PlayerRole role;
    public PowerUpScriptableObject currentPowerUp { get; set; }
    private int castNumber = 1;
    public PlayerRole Role { get => role; }

    private PlayerInputAction playerInput;

    private void Awake() {
        playerInput = new PlayerInputAction();
    }

    private void UsePowerUp(InputAction.CallbackContext ctx) {
        if (!IsOwner) return;

        ApplyPowerUp();
        ApplyPowerUpServerRPC();
    }

    private void ApplyPowerUp() {
        if (currentPowerUp != null) {
            currentPowerUp.ApplyPowerUp(transform, castNumber);
            if (castNumber == currentPowerUp.numberOfCasts) {
                castNumber = 1;
                currentPowerUp = null;
                return;
            }
            castNumber++;
        }
    }

    [ServerRpc]
    void ApplyPowerUpServerRPC() {
        ApplyPowerUpClientRPC();
    }

    [ClientRpc]
    void ApplyPowerUpClientRPC() {
        if (IsLocalPlayer) return;

        ApplyPowerUp();
    }

    private void OnEnable() {
        playerInput.Enable();
        playerInput.Abilities.ApplyPowerUp.performed += UsePowerUp;
    }

    private void OnDisable() {
        playerInput.Disable();
        playerInput.Abilities.ApplyPowerUp.performed -= UsePowerUp;
    }
}
