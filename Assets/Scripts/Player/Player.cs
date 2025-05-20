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
        [SerializeField] private float airControlMultiplier = 0.4f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float jumpCooldown = 0.25f;
        [SerializeField] private float gravity = -18f;
        private bool _readyToJump = true;

        [Header("Crouching")]
        private float _startYScale;
        [SerializeField] private float crouchYScale = 0.5f;
        [SerializeField] private float transitionDuration = 0.1f;
        private Coroutine _currentTransition;

        [SerializeField] private Transform orientation;

        private float _horizontalInput;
        private float _verticalInput;
        private Vector3 _moveDirection;
        private Vector3 _currentVelocity;
        private float _verticalVelocity;

        private CharacterController _controller;

        public enum MovementState { Sprinting, Crouching, Air }
        public MovementState state;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _startYScale = transform.localScale.y;
        }

        private void Update()
        {
            HandleInput();
            StateHandler();
            HandleJump();
            ApplyGravity();
            HandleMovement();
        }

        private void HandleInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (_currentTransition != null)
                    StopCoroutine(_currentTransition);
                _currentTransition = StartCoroutine(SmoothScaleChange(crouchYScale));
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (_currentTransition != null)
                    StopCoroutine(_currentTransition);
                _currentTransition = StartCoroutine(SmoothScaleChange(_startYScale));
            }
        }

        private void HandleMovement()
        {
            _moveDirection = (orientation.forward * _verticalInput + orientation.right * _horizontalInput).normalized;
            float targetSpeed = (state == MovementState.Crouching) ? crouchSpeed : sprintSpeed;
            Vector3 desiredVelocity = _moveDirection * targetSpeed;

            // Aplica air control multiplier si está en el aire
            float effectiveAcceleration = acceleration;
            float effectiveDeceleration = deceleration;
            if (!_controller.isGrounded)
            {
                effectiveAcceleration *= airControlMultiplier;
                effectiveDeceleration *= airControlMultiplier;
            }

            // Suavizado de aceleración/desaceleración en plano XZ
            var flatCurrentVel = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);
            var flatDesiredVel = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);

            flatCurrentVel = _moveDirection.magnitude > 0.1f ? Vector3.MoveTowards(flatCurrentVel, flatDesiredVel, effectiveAcceleration * Time.deltaTime) : Vector3.MoveTowards(flatCurrentVel, Vector3.zero, effectiveDeceleration * Time.deltaTime);

            _currentVelocity = new Vector3(flatCurrentVel.x, _currentVelocity.y, flatCurrentVel.z);

            // Combina el movimiento horizontal con el vertical (gravedad/jump)
            Vector3 finalVelocity = new Vector3(flatCurrentVel.x, _verticalVelocity, flatCurrentVel.z);
            _controller.Move(finalVelocity * Time.deltaTime);
        }


        private void HandleJump()
        {
            if (_controller.isGrounded && _verticalVelocity < 0)
                _verticalVelocity = -2f;

            if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded && state != MovementState.Crouching && _readyToJump)
            {
                _verticalVelocity = jumpForce;
                _readyToJump = false;
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void ApplyGravity()
        {
            _verticalVelocity += gravity * Time.deltaTime;
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
            if (Input.GetKey(KeyCode.LeftControl))
                state = MovementState.Crouching;
            else if (_controller.isGrounded)
                state = MovementState.Sprinting;
            else
                state = MovementState.Air;
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }
    }
}