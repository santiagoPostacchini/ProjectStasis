using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    #region Movement Variables
    [Header("<color=orange>Movement Values</color>")]
    [Tooltip("Modifies how fast the player will move.")]
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _height = 2f;
    [SerializeField] private float _airMultiplier = 0.5f;
    [SerializeField] private float _groundDrag = 5f;
    private float _movSpeed;
    private bool _readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode walkKey = KeyCode.LeftShift;
    // Teleport key is handled by TeleportJumpWeapon, so it is not used here.

    [Header("Ground Check")]
    public LayerMask groundLayer;

    [Header("Crouching")]
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _crouchYScale = 0.5f;
    [SerializeField] private float _startYScale = 1f;

    [Header("Crouch Fatigue")]
    [SerializeField] private float crouchFatigueIncreaseRate = 1f;
    [SerializeField] private float crouchFatigueRecoveryRate = 0.5f;
    [SerializeField] private float maxCrouchFatigue = 3f;
    [SerializeField] private float extraCrouchAmount = 0.2f;
    [SerializeField] private float crouchLerpSpeed = 5f;

    private float crouchFatigue = 0f;
    private Vector3 targetScale;
    #endregion

    #region Movement and State Components
    public Transform orientation;
    public enum MovementStates { Walking, Sprinting, Air, Crouching }
    public MovementStates state;
    private float _xAxis = 0f, _zAxis = 0f;
    private Vector3 _dir = Vector3.zero;
    private Rigidbody _rb;
    private bool _grounded, _isMoving = false;
    public static Player Instance;
    #endregion

    // (Assuming PlayerInteractor is used for picking up objects.)
    private PlayerInteractor playerInteractor;

    // Reference to the TeleportJumpWeapon component.
    private TeleportJumpWeapon teleportWeapon;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody>();
        _rb.angularDrag = 1f;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _startYScale = transform.localScale.y;
        targetScale = transform.localScale;
    }

    private void Start()
    {
        playerInteractor = GetComponentInChildren<PlayerInteractor>();

        // Get the TeleportJumpWeapon component from this GameObject.
        teleportWeapon = GetComponent<TeleportJumpWeapon>();
        if (teleportWeapon == null)
        {
            Debug.LogWarning("Player: TeleportJumpWeapon component not found on the GameObject.");
        }
    }

    private void Update()
    {
        //_grounded = Physics.Raycast(transform.position, Vector3.down, _height * 0.5f + 0.2f, groundLayer);
        _grounded = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, _height * 0.5f + 0.2f, groundLayer);

        Controller();
        StateHandler();

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchLerpSpeed * Time.deltaTime);
        _rb.drag = _grounded ? _groundDrag : 0f;

        // Handle teleport input.
        // When the T key is pressed, delegate to the TeleportJumpWeapon component.
        if (Input.GetKeyDown(KeyCode.T) && teleportWeapon != null)
        {
            teleportWeapon.ActivateTeleport();
        }
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Movement(_xAxis, _zAxis);
            SpeedControl();
        }
    }

    #region Movement Methods
    void Controller()
    {
        _xAxis = Input.GetAxis("Horizontal");
        _zAxis = Input.GetAxis("Vertical");

        _isMoving = (_xAxis != 0 || _zAxis != 0);

        if (Input.GetKeyDown(jumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            Jump();
            ResetJump();
        }
    }

    void StateHandler()
    {
        if (_grounded && Input.GetKey(walkKey))
        {
            state = MovementStates.Walking;
            _movSpeed = _walkSpeed;
        }
        else if (_grounded)
        {
            state = MovementStates.Sprinting;
            _movSpeed = _sprintSpeed;
        }
        else
        {
            state = _rb.velocity.y > 0.1f ? MovementStates.Air : MovementStates.Sprinting;
        }

        if (Input.GetKey(crouchKey))
        {
            state = MovementStates.Crouching;
            crouchFatigue += Time.deltaTime * crouchFatigueIncreaseRate;
            crouchFatigue = Mathf.Clamp(crouchFatigue, 0f, maxCrouchFatigue);
            float extraCrouch = Mathf.Lerp(0f, extraCrouchAmount, crouchFatigue / maxCrouchFatigue);
            targetScale = new Vector3(transform.localScale.x, _crouchYScale - extraCrouch, transform.localScale.z);
            _movSpeed = _crouchSpeed;
        }
        else
        {
            crouchFatigue -= Time.deltaTime * crouchFatigueRecoveryRate;
            crouchFatigue = Mathf.Max(crouchFatigue, 0f);
            targetScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (flatVel.magnitude > _movSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _movSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void Movement(float xAxis, float zAxis)
    {
        _dir = (orientation.forward * zAxis + orientation.right * xAxis).normalized;
        if (_grounded)
            _rb.AddForce(_dir * _movSpeed * 10f, ForceMode.Force);
        else
            _rb.AddForce(_dir * _movSpeed * 10f * _airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
    #endregion
}