using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerContext : MonoBehaviour {
    private PlayerInputAction playerInput;
    private Rigidbody rb;
    private Animator anim;
    private Vector2 inputVector;

    [SerializeField] private Camera playerCamera;

    #region Variables
    [Header("References")]
    public Transform orientation;
    public Transform visuals;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float groundDrag;

    [Header("Ground Detection")]
    [SerializeField] private float checkSphereRadius;
    [SerializeField] private Transform feet;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region Properties
    public Animator Anim { get => anim; }
    public Rigidbody RB { get => rb; }
    public float MoveSpeed { get => moveSpeed; }
    public float AirMultiplier { get => airMultiplier; }
    public float SpeedMultiplier { get => speedMultiplier; }
    public float JumpForce { get => jumpForce; }
    public float RotationSpeed { get => rotationSpeed; }
    public float DashCooldown { get; set; }
    public float CurrentSpeed { get; set; }
    public float DesiredSpeed { get; set; }
    public Vector3 MoveDir { get; set; }
    public bool IsJumpPressed { get; set; }
    public bool RequireNewJumpPress { get; set; }
    public bool IsSwingPressed { get; set; }
    public bool RequireNewSwingPress { get; set; }
    public bool IsDashingPressed { get; set; }
    public bool IsGrounded { get; set; }
    public bool IsMoving { get; set; }
    #endregion

    #region State Machine
    public BaseState CurrentState { get; set; }
    private StateFactory stateFactory;
    #endregion

    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        stateFactory = new StateFactory(this);
        CurrentState = stateFactory.Ground();
        CurrentState.EnterState();

        playerInput = new PlayerInputAction();
        playerInput.Movement.Jump.started += OnJump;
        playerInput.Movement.Jump.canceled += OnJump;
        playerInput.Abilities.Dash.started += OnDash;
        playerInput.Abilities.Dash.canceled += OnDash;
        playerInput.Abilities.Swing.started += OnSwing;
        playerInput.Abilities.Swing.canceled += OnSwing;
    }

    private void Start() {
        CurrentSpeed = MoveSpeed;
    }

    private void Update() {
        HandleRotation();

        inputVector = GetMovement();
        MoveDir = orientation.forward * inputVector.y + orientation.right * inputVector.x;
        IsMoving = inputVector != Vector2.zero;

        CurrentState.UpdateStates();

        if (IsGrounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0f;
        }

        // cooldowns
        DashCooldown -= Time.deltaTime;
    }

    private void FixedUpdate() {
        IsGrounded = Physics.CheckSphere(feet.position, checkSphereRadius, groundLayer);

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

        if (velocity.magnitude > CurrentSpeed) {
            Vector3 limitedVel = velocity.normalized * CurrentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
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
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}
