using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("<color=orange>Movement values</color>")]
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

    bool _grounded, _isMoving = false;
    public MovementStates state;

    public enum MovementStates
    {
        Walking,
        Sprinting,
        Air,
        Crouching
    }

    public Transform orientation;
    private float _xAxis = 0f, _zAxis = 0f;
    private Vector3 _dir = new Vector3();
    private Rigidbody _rb;
    public static Player Instance;

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody>();
        _rb.angularDrag = 1f;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _startYScale = transform.localScale.y;
        targetScale = transform.localScale;
    }

    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, _height * 0.5f + 0.2f, groundLayer);

        Controller();
        StateHandler();

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, crouchLerpSpeed * Time.deltaTime);

        _rb.drag = _grounded ? _groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Movement(_xAxis, _zAxis);
            SpeedControl();
        }
    }

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
            state = MovementStates.Air;
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
}
