using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;


public class Swing : NetworkBehaviour
{
    private PlayerInputAction playerInputAction;
    private Transform hitTransform;
    private Rigidbody rb;
    private Vector3 swingDirection;
    private PlayerContext playerContext;
    private Camera playerCam;

    [Header("Swinging References")]
    [SerializeField] private float swingForce;
    [SerializeField] private float swingSpeed;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxSwingDistance;
    [SerializeField] private float sphereCastRadius;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {
            this.enabled = false;
            return;
        }
        playerContext = GetComponent<PlayerContext>();
        playerCam = playerContext.GetPlayerCamera();
        rb = GetComponent<Rigidbody>();
        playerInputAction = new();
        playerInputAction.Enable();
        playerInputAction.Abilities.Swing.performed += OnSwingPerformed;
    }

    private void Update() {
        RaycastHit hitInfo;
        if (Physics.SphereCast(this.transform.position, sphereCastRadius, playerCam.transform.forward, out hitInfo, maxSwingDistance, layerMask)) {     
            hitTransform = hitInfo.collider.transform;
            hitTransform.GetComponent<SwingableObject>().Visual.GetComponent<SwingableObjectUI>().playerCamera = playerContext.GetPlayerCamera();
            hitTransform.GetComponent<SwingableObject>().Visual.gameObject.SetActive(true);
        }
        else {
            if (hitTransform != null) {
                hitTransform.GetComponent<SwingableObject>().Visual.gameObject.SetActive(false);

            }
            hitTransform = null;
        }
    }
    private void OnSwingPerformed(InputAction.CallbackContext context)
    {
        if (hitTransform != null) {
            swingDirection = hitTransform.GetComponent<SwingableObject>().target.position - this.transform.position; 
            rb.AddForce(swingDirection * swingSpeed, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(this.transform.position, playerCam.transform.forward * maxSwingDistance);
    }
}
