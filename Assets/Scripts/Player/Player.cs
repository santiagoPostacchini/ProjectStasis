using System.Collections;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 30f;
        [SerializeField] private float groundDrag = 6f;
        [SerializeField] private float crouchDrag = 8f;  // Mayor drag al agacharse para mejor control
        [SerializeField] private float airDrag = 0.5f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float jumpCooldown = 0.25f;
        [SerializeField] private float airMultiplier = 0.8f;
        private bool _readyToJump = true;

        [Header("Crouching")]
        private float _startYScale;
        [SerializeField] private float crouchYScale = 0.5f;
        [SerializeField] private float transitionDuration = 0.1f;

        [Header("Step Climb")]
        [SerializeField] private float stepSmooth = 0.1f;
        [SerializeField] private float stepHeight = 0.5f;
        [Tooltip("Lower raycast Transform (place near the player's feet, slightly in front)")]
        public Transform stepRayLower;
        [Tooltip("Upper raycast Transform (place at a max step height, same x/z as lower)")]
        public Transform stepRayUpper;

        [Header("Custom Gravity")]
        [SerializeField] private float gravity = -18f;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

        [Header("Ground Check")]
        public LayerMask whatIsGround;
        [SerializeField] private CapsuleCollider body;
        private bool _grounded;
        private float _playerHeight;

        [Header("Slope Handling")]
        [SerializeField] private float maxSlopeAngle = 45f;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;

        [SerializeField] private Transform orientation;

        private float _horizontalInput;
        private float _verticalInput;
        private Vector3 _moveDirection;
        private Vector3 _desiredVelocity;
        private Vector3 _currentVelocity;

        private Rigidbody _rb;
        private Coroutine _currentTransition;

        public enum MovementState
        {
            Sprinting,
            Crouching,
            Air
        }
        public MovementState state;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.useGravity = false;
            _readyToJump = true;
            _startYScale = transform.localScale.y;
            _playerHeight = body.height;
            
            stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);
        }

        private void Update()
        {
            // Sphere cast to check if the player is grounded
            _grounded = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out _, _playerHeight * 0.25f + 0.2f, whatIsGround);
            
            MyInput();
            StateHandler();

            // Establecer el drag apropiado según el estado
            if (!_grounded)
                _rb.drag = airDrag;
            else if (state == MovementState.Crouching)
                _rb.drag = crouchDrag;   // Más alto que groundDrag
            else
                _rb.drag = groundDrag;
        }

        private void FixedUpdate()
        {
            MovePlayer();
            SpeedControl();
            ApplyGravity();
            StepClimb();
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKeyDown(jumpKey) && _readyToJump && _grounded && state != MovementState.Crouching)
            {
                _readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }

            // Crouch
            if (Input.GetKeyDown(crouchKey))
            {
                if (_currentTransition != null)
                    StopCoroutine(_currentTransition);
                _currentTransition = StartCoroutine(SmoothScaleChange(crouchYScale));

                if (_grounded)
                    _rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
                
                Vector3 flatVel = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                float maxCrouch = crouchSpeed;
                if (flatVel.magnitude > maxCrouch)
                {
                    flatVel = flatVel.normalized * maxCrouch;
                    _rb.velocity = new Vector3(flatVel.x, _rb.velocity.y, flatVel.z);
                }
            }
            else if (Input.GetKeyUp(crouchKey))
            {
                if (!Physics.Raycast(transform.position, Vector3.up, _playerHeight, whatIsGround))
                {
                    if (_currentTransition != null)
                        StopCoroutine(_currentTransition);
                    _currentTransition = StartCoroutine(SmoothScaleChange(_startYScale));
                }
            }
        }

        private IEnumerator SmoothScaleChange(float targetYScale)
        {
            float elapsed = 0f;
            Vector3 initialScale = transform.localScale;
            Vector3 targetScale = new Vector3(initialScale.x, targetYScale, initialScale.z);

            while (elapsed < transitionDuration)
            {
                transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / transitionDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }

        private void StateHandler()
        {
            if (Input.GetKey(crouchKey))
            {
                state = MovementState.Crouching;
            }
            else if (_grounded)
            {
                state = MovementState.Sprinting;
            }
            else
            {
                state = MovementState.Air;
            }
        }

        private void MovePlayer()
        {
            _moveDirection = (orientation.forward * _verticalInput + orientation.right * _horizontalInput).normalized;
            
            if (_horizontalInput == 0 && _verticalInput == 0)
                _moveDirection = Vector3.zero;
        
            float targetSpeed = (state == MovementState.Crouching) ? crouchSpeed : sprintSpeed;
            _desiredVelocity = _moveDirection * targetSpeed;

            _currentVelocity = _moveDirection.magnitude > 0.1f ? 
                Vector3.MoveTowards(_currentVelocity, _desiredVelocity, acceleration * Time.fixedDeltaTime) : 
                Vector3.MoveTowards(_currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);

            Vector3 moveVel = _currentVelocity;
            
            if (OnSlope() && !_exitingSlope)
            {
                _rb.AddForce(GetSlopeMoveDirection() * (moveVel.magnitude * 20f), ForceMode.Force);
            }
            else if (_grounded)
            {
                _rb.AddForce(moveVel * 10f, ForceMode.Force);
            }
            else
            {
                _rb.AddForce(moveVel * (airMultiplier * 10f), ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            float maxSpeed = (state == MovementState.Crouching) ? crouchSpeed : sprintSpeed;

            if (flatVel.magnitude > maxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * maxSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }

        private void StepClimb()
        {
            if (Physics.Raycast(stepRayLower.position, transform.TransformDirection(Vector3.forward), out _, 0.1f))
            {
                if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(Vector3.forward), out _, 0.2f))
                {
                    _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            if (Physics.Raycast(stepRayLower.position, transform.TransformDirection(1.5f,0,1), out _, 0.1f))
            {
                if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(1.5f,0,1), out _, 0.2f))
                {
                    _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            if (!Physics.Raycast(stepRayLower.position, transform.TransformDirection(-1.5f, 0, 1),
                    out _, 0.1f)) return;

            if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(-1.5f,0,1), out _, 0.2f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        private void Jump()
        {
            _exitingSlope = true;
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        private void ResetJump()
        {
            _readyToJump = true;
        }
        
        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
            return false;
        }
        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
        }

        private void ApplyGravity()
        {
            _rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }
}
