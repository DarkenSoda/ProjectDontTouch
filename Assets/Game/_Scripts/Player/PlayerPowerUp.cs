using Scripts.PowerUps;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerUp : NetworkBehaviour {
    [SerializeField] private PlayerRole role;
    public PowerUpBehaviour CurrentPowerUp { get; set; }
    public int CastNumber { get; private set; } = 1;
    public PlayerRole Role { get => role; }

    private PlayerInputAction playerInput;

    private void Awake() {
        playerInput = new PlayerInputAction();
    }

    private void UsePowerUp(InputAction.CallbackContext ctx) {
        if (!IsOwner) return;

        ApplyPowerUp();
        // ApplyPowerUpServerRPC();
    }

    private void ApplyPowerUp() {
        if (CurrentPowerUp != null) {
            CurrentPowerUp.ApplyPowerUp();
        }
    }

    public void UpdateCounter() {
        if (CastNumber == CurrentPowerUp.numberOfCasts) {
            ResetCounter();
            CurrentPowerUp = null;
            return;
        }
        CastNumber++;
    }

    public void ResetCounter() {
        CastNumber = 1;
    }

    public void SetPowerUpUser() {
        if (CurrentPowerUp != null) {
            CurrentPowerUp.Player = this;
        }
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
