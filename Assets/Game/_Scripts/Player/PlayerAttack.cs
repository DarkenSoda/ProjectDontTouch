using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour {
    [Header("Attack")]
    [SerializeField] private float maxAttackDistance;

    private PlayerInputAction playerInput;

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            this.enabled = false;
            return;
        }
    }

    private void Awake() {
        playerInput = new PlayerInputAction();
        playerInput.Abilities.Attack.performed += OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext ctx) {
        AttackServerRpc();
    }

    [ServerRpc]
    private void AttackServerRpc() {
        if (GetComponent<PlayerPowerUp>().Role.Value == PlayerRole.Tagger) {
            if (Physics.Raycast(transform.position, GetComponent<PlayerContext>().cameraForward.Value, out RaycastHit hitInfo, maxAttackDistance)) {
                if (hitInfo.transform.GetComponent<PlayerPowerUp>().Role.Value == PlayerRole.Runner) {
                    RoundManager.Instance.KillPlayer(hitInfo.transform.GetComponent<NetworkObject>().OwnerClientId);
                }
            }
        }
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}
