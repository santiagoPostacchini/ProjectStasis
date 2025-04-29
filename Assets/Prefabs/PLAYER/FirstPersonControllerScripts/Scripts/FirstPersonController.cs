/*
FirstPersonController Configurable Properties:

// MovementSettings (ScriptableObject)
// MouseSensitivity (float): Sensibilidad del ratón para rotar la cámara.
// Snappiness (float): Rapidez de suavizado de la cámara (no usado actualmente).
// WalkSpeed (float): Velocidad al andar.
// SprintSpeed (float): Velocidad al esprintar.
// CrouchSpeed (float): Velocidad al agacharse.
// SlideSpeed (float): Velocidad de deslizamiento.
// SlideDuration (float): Duración en segundos del deslizamiento.
// SlideFovBoost (float): Aumento de FOV durante el slide.
// SlideTiltAngle (float): Ángulo de inclinación de cámara en el slide.
// JumpHeight (float): Altura del salto en metros.
// Gravity (float, negativo): Intensidad de la gravedad aplicada.
// AirControl (float): Control de movimiento en el aire (0 = ninguno, 1 = completo).
// CoyoteTimeEnabled (bool): Permite saltar brevemente tras perder contacto con el suelo.
// CoyoteTimeDuration (float): Tiempo en segundos para usar coyote time.
// NormalFov (float): Campo de visión estándar.
// SprintFov (float): FOV al esprintar.
// FovChangeSpeed (float): Velocidad de interpolación del FOV.
// BobAmount (float): Amplitud del "head bob" al caminar.
// BobSpeed (float): Velocidad del "head bob".
// CrouchCamHeight (float): Altura de la cámara al agacharse o deslizarse.
// GroundDistance (float): Radio de la esfera para detectar el suelo.

// FirstPersonController (MonoBehaviour)
// _groundCheck (Transform): Punto de chequeo de suelo.
// _groundMask (LayerMask): Capas consideradas como suelo.
// _cameraParent (Transform): Padre de la cámara usado en head bob.
// _playerCamera (Transform): Transform de la cámara para rotaciones y FOV.
// _jumpCooldown (float): Tiempo mínimo entre saltos consecutivos (en segundos).
*/

using UnityEngine;
using System.Collections;

public enum PlayerState { Grounded, Airborne, Crouching, Sliding }

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Config SO")]
    [SerializeField] private MovementSettings _settings;

    [Header("References")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Transform _playerCamera;

    [Header("Jump Cooldown")]
    [SerializeField] private float _jumpCooldown = 0.2f;
    private float _lastJumpTime = -Mathf.Infinity;

    private CharacterController _cc;
    private AudioSource _slideAudio;
    private Camera _cam;

    private float _rotX, _rotY;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _canMove = true, _canLook = true;
    private float _coyoteTimer;
    private float _originalHeight, _originalCamHeight;
    private float _bobTimer;

    private PlayerState _state = PlayerState.Grounded;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _slideAudio = GetComponent<AudioSource>();
        _cam = _playerCamera.GetComponent<Camera>();
        _originalHeight = _cc.height;
        _originalCamHeight = _cameraParent.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (_canLook) HandleLook();
        ReadInput();
        HandleStateTransitions();
        UpdateFov();
        UpdateHeadBob();
    }

    private void FixedUpdate()
    {
        if (_canMove) HandleMovement();
        ApplyGravity();
    }

    private void HandleLook()
    {
        float mx = Input.GetAxis("Mouse X") * _settings.MouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * _settings.MouseSensitivity * Time.deltaTime;

        _rotX += mx;
        _rotY = Mathf.Clamp(_rotY - my, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, _rotX, 0f);
        float tilt = (_state == PlayerState.Sliding) ? _settings.SlideTiltAngle : 0f;
        _playerCamera.localRotation = Quaternion.Euler(_rotY, 0f, tilt);
    }

    private void ReadInput()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        HandleJumpInput();
        HandleSlideInput();
    }

    private void HandleJumpInput()
    {
        bool canUseJump = Time.time - _lastJumpTime >= _jumpCooldown;
        if (Input.GetKeyDown(KeyCode.Space) && canUseJump && _state == PlayerState.Grounded &&
            (_coyoteTimer > 0f || IsGrounded()))
        {
            Jump();
        }
    }

    private void HandleSlideInput()
    {
        if (_state == PlayerState.Grounded && Input.GetKeyDown(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) && _moveInput.y > 0.1f)
        {
            StartCoroutine(SlideRoutine());
        }
    }


    private void HandleStateTransitions()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            _state = Input.GetKey(KeyCode.LeftControl) ? PlayerState.Crouching : PlayerState.Grounded;
            _velocity.y = _settings.GroundDistance * -10f; // Anclar al suelo
            _coyoteTimer = _settings.CoyoteTimeEnabled ? _settings.CoyoteTimeDuration : 0f;
        }
        else if (_state != PlayerState.Sliding)
        {
            _state = PlayerState.Airborne;
            _coyoteTimer -= Time.deltaTime; // Reducir el coyote timer solo si no está en el suelo
        }
    }

    private void HandleMovement()
    {
        if (_state == PlayerState.Sliding) return;

        Vector3 direction = (transform.right * _moveInput.x + transform.forward * _moveInput.y).normalized;
        float speed = CalculateSpeed();

        Vector3 totalMove = direction * speed + new Vector3(0f, _velocity.y, 0f);
        _cc.Move(totalMove * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Calcula la velocidad base del jugador según su estado.
    /// </summary>
    private float CalculateSpeed()
    {
        if (_state == PlayerState.Crouching) return _settings.CrouchSpeed;
        if (_state == PlayerState.Airborne) return _settings.WalkSpeed * _settings.AirControl;

        bool sprinting = Input.GetKey(KeyCode.LeftShift) && _moveInput.y > 0.1f;
        return sprinting ? _settings.SprintSpeed : _settings.WalkSpeed;
    }

    private void ApplyGravity()
    {
        // Verificar si el jugador está en el suelo
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            // Anclar al suelo con un valor configurable
            _velocity.y = _settings.GroundDistance * -10f; // Valor configurable para evitar "flotación"
        }
        else
        {
            // Aplicar gravedad solo si no está en el suelo
            _velocity.y += _settings.Gravity * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Verifica si el jugador está en contacto con el suelo.
    /// </summary>
    /// <returns>True si está en el suelo, de lo contrario False.</returns>
    private bool IsGrounded()
    {
        return Physics.CheckSphere(_groundCheck.position, _settings.GroundDistance, _groundMask);
    }

    private void Jump()
    {
        _lastJumpTime = Time.time;
        _velocity.y = Mathf.Sqrt(
            _settings.JumpHeight * -2f * _settings.Gravity
        );
        _coyoteTimer = 0f;
        _state = PlayerState.Airborne;
    }

    private IEnumerator SlideRoutine()
    {
        _state = PlayerState.Sliding;
        float timer = _settings.SlideDuration;
        Vector3 dir = (transform.right * _moveInput.x +
                       transform.forward * _moveInput.y).normalized;
        if (dir.magnitude < 0.1f) dir = transform.forward;

        while (timer > 0f && Physics.CheckSphere(
                   _groundCheck.position,
                   _settings.GroundDistance,
                   _groundMask))
        {
            float t = timer / _settings.SlideDuration;
            float s = Mathf.Lerp(
                _settings.SlideSpeed * 0.5f,
                _settings.SlideSpeed,
                t * t
            );
            _cc.Move(dir * s * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        _state = PlayerState.Grounded;
    }

    private void UpdateFov()
    {
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float target = (_state == PlayerState.Sliding)
            ? _settings.SprintFov + _settings.SlideFovBoost
            : (sprinting ? _settings.SprintFov : _settings.NormalFov);

        _cam.fieldOfView = Mathf.Lerp(
            _cam.fieldOfView,
            target,
            Time.deltaTime * _settings.FovChangeSpeed
        );
    }

    private void UpdateHeadBob()
    {
        float targetY = (_state == PlayerState.Crouching || _state == PlayerState.Sliding)
            ? _settings.CrouchCamHeight
            : _originalCamHeight;

        if (_moveInput.magnitude > 0f && _state == PlayerState.Grounded)
        {
            _bobTimer += Time.deltaTime * _settings.BobSpeed * (_state == PlayerState.Crouching ? _settings.CrouchSpeed : _settings.WalkSpeed);
            targetY += Mathf.Sin(_bobTimer) * _settings.BobAmount;
        }
        else
        {
            _bobTimer = 0f;
        }

        Vector3 localPosition = _cameraParent.localPosition;
        localPosition.y = Mathf.Lerp(localPosition.y, targetY, Time.deltaTime * 15f);
        _cameraParent.localPosition = localPosition;
    }


    /// <summary>
    /// Activa o desactiva rotación y movimiento.
    /// </summary>
    public void SetControl(bool look, bool move)
    {
        _canLook = look;
        _canMove = move;
    }

    /// <summary>
    /// Controla la visibilidad y bloqueo del cursor.
    /// </summary>
    public void SetCursorVisibility(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
