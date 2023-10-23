using Scripts.PowerUps;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerUp : NetworkBehaviour {
    public Transform TEST;
    [SerializeField] private PlayerRole role;
    public PowerUpScriptableObject currentPowerUp { get; set; }
    private int castNumber = 1;
    public PlayerRole Role { get => role; }

    private PlayerInputAction playerInput;

    public override void OnNetworkSpawn() {
        if (!IsOwner) enabled = false;
    }

    private void Awake() {
        playerInput = new PlayerInputAction();
    }

    private void UsePowerUp(InputAction.CallbackContext ctx) {
        if (!IsOwner) return;

        if (currentPowerUp != null) {
            ApplyPowerUpClientRPC();
            if (castNumber == currentPowerUp.numberOfCasts) {
                castNumber = 1;
                currentPowerUp = null;
                return;
            }
            castNumber++;
        }
    }

    [ClientRpc]
    private void ApplyPowerUpClientRPC() {
        currentPowerUp.ApplyPowerUp(transform, castNumber);
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
