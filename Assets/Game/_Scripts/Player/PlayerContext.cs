using System;
using System.Collections;
using Cinemachine;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerContext : NetworkBehaviour {
    private PlayerInputAction playerInput;
    private Rigidbody rb;
    private Animator anim;
    private Vector2 inputVector;
    public RaycastHit WallHit;

    [SerializeField] private Camera playerCamera;

    #region Variables
    [Header("References")]
    public Transform orientation;
    public Transform visuals;

    [Header("Momentum Speeds")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float minSpeedDifference;

    [Header("Movement")]
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float groundDrag;

    [Header("Dash")]
    [SerializeField] private float dashPower;
    [SerializeField] private float verticalDashPower;
    [SerializeField] private float dashCooldownMax;
    [SerializeField] private float dashDuration;

    [Header("Wall Stand & Wall Run")]
    [SerializeField] private float minGroundDistance;
    [SerializeField] private float wallStandAngle;
    [SerializeField] private float wallDetectionRadius;
    [SerializeField] private float wallDetectionDistance;
    [SerializeField] private float slidingGravity;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private LayerMask wallLayer;

    [Header("Ground Detection")]
    [SerializeField] private float checkSphereRadius;
    [SerializeField] private Transform feet;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region Propertiesw
    public Animator Anim { get => anim; }
    public Rigidbody RB { get => rb; }
    public float MoveSpeed { get => moveSpeed; }
    public float AirMultiplier { get => airMultiplier; }
    public float SpeedMultiplier { get => speedMultiplier; }
    public float JumpForce { get => jumpForce; }
    public float GroundDrag { get => groundDrag; }
    public float DashSpeed { get => dashSpeed; }
    public float DashPower { get => dashPower; }
    public float DashDuration { get => dashDuration; }
    public float VerticalDashPower { get => verticalDashPower; }
    public float RotationSpeed { get => rotationSpeed; }
    public float DashCooldownMax { get => dashCooldownMax; }
    public float SlidingGravity { get => slidingGravity; }
    public float WallJumpForce { get => wallJumpForce; }
    public float WallStandAngle { get => wallStandAngle; }
    public float WallRunSpeed { get => wallRunSpeed; }
    public float WallDetectionDistance { get => wallDetectionDistance; }
    public LayerMask WallLayer { get => wallLayer; }
    public float DashCooldown { get; set; }
    public float CurrentSpeed { get; set; }
    public float DesiredSpeed { get; set; }
    public float LastDesiredSpeed { get; set; }
    public float AngleBetweenWall { get; set; }
    public Vector3 MoveDir { get; set; }
    public bool IsJumpPressed { get; set; }
    public bool RequireNewJumpPress { get; set; }
    public bool IsSwingPressed { get; set; }
    public bool RequireNewSwingPress { get; set; }
    public bool IsDashingPressed { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsFarFromGround { get; set; }
    public bool IsNearWall { get; set; }
    public bool IsMoving { get; set; }
    public bool ApplyGravity { get; set; }
    public bool ApplyRotation { get; set; }
    #endregion

    #region State Machine
    public BaseState CurrentState { get; set; }
    private StateFactory stateFactory;
    #endregion

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            this.enabled = false;
            return;
        }

        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        Camera.main.gameObject.GetComponent<AudioListener>().enabled = false;
        playerCamera.gameObject.GetComponent<AudioListener>().enabled = true;
        ThirdPersonCamera thirdPersonCamera = playerCamera.gameObject.GetComponent<ThirdPersonCamera>();
        thirdPersonCamera.player = this;
        thirdPersonCamera.freeLookCamera.Follow = transform;
        thirdPersonCamera.freeLookCamera.LookAt = transform;
        playerCamera.depth = 1;
    }

    private void Awake() {
        playerInput = new PlayerInputAction();
        playerInput.Movement.Jump.started += OnJump;
        playerInput.Movement.Jump.canceled += OnJump;
        playerInput.Abilities.Dash.started += OnDash;
        playerInput.Abilities.Dash.canceled += OnDash;
        playerInput.Abilities.Swing.started += OnSwing;
        playerInput.Abilities.Swing.canceled += OnSwing;
    }

    private void Start() {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        stateFactory = new StateFactory(this);
        CurrentState = stateFactory.Ground();
        CurrentState.EnterState();

        ApplyGravity = true;
        ApplyRotation = true;
    }

    private void Update() {
        IsGrounded = Physics.CheckSphere(feet.position, checkSphereRadius, groundLayer);

        if (ApplyRotation) {
            HandleRotation();
        }

        rb.useGravity = ApplyGravity;

        SpeedControl();

        inputVector = GetMovement();
        MoveDir = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        IsMoving = inputVector != Vector2.zero;

        // cooldowns
        DashCooldown -= Time.deltaTime;


        CurrentState.UpdateStates();

        if (Mathf.Abs(DesiredSpeed - LastDesiredSpeed) > minSpeedDifference && CurrentSpeed != 0) {
            StopCoroutine(SmoothlyLerpSpeed());
            StartCoroutine(SmoothlyLerpSpeed());
        } else {
            CurrentSpeed = DesiredSpeed;
        }

        LastDesiredSpeed = DesiredSpeed;


        IsFarFromGround = !Physics.Raycast(transform.position, Vector3.down, minGroundDistance, groundLayer);
        IsNearWall = Physics.SphereCast(transform.position, wallDetectionRadius, visuals.forward, out WallHit, wallDetectionDistance, wallLayer);
        
        if(IsNearWall) {
            AngleBetweenWall = Mathf.Abs(Vector3.Angle(-WallHit.normal, visuals.forward));
        }
    }

    private void FixedUpdate() {
        CurrentState.FixedUpdateStates();
    }

    private void HandleRotation() {
        Vector2 inputVector = GetMovement();
        Vector3 inputDir = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        if (IsMoving) {
            visuals.forward = Vector3.Slerp(visuals.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);
        }
    }

    private void SpeedControl() {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (velocity.magnitude > MoveSpeed) {
            Vector3 limitedVel = velocity.normalized * MoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private IEnumerator SmoothlyLerpSpeed() {
        float time = 0;
        float difference = Mathf.Abs(DesiredSpeed - CurrentSpeed);
        float startValue = CurrentSpeed;

        while (time < difference) {
            CurrentSpeed = Mathf.Lerp(startValue, DesiredSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        CurrentSpeed = DesiredSpeed;
    }

    public Vector2 GetMovement() {
        return playerInput.Movement.Move.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx) {
        IsJumpPressed = ctx.ReadValueAsButton();
        RequireNewJumpPress = false;
    }

    private void OnSwing(InputAction.CallbackContext ctx) {
        IsSwingPressed = ctx.ReadValueAsButton();
        RequireNewSwingPress = false;
    }

    private void OnDash(InputAction.CallbackContext ctx) {
        IsDashingPressed = ctx.ReadValueAsButton();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(feet.position, checkSphereRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * minGroundDistance);
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}
