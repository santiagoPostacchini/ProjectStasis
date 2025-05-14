// PlayerMovement.cs
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IMovementModule
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad al caminar (mantener Shift para caminar lento)")] public float walkSpeed = 10f;
    [Tooltip("Velocidad al correr por defecto")] public float sprintSpeed = 15f;
    [Tooltip("Aceleración de la gravedad")] public float gravity = 9.81f;
    [Tooltip("Impulso de salto")] public float jumpSpeed = 3f;
    [Tooltip("Permite sprint")] public bool canSprint = true;
    [Tooltip("Permite salto")] public bool canJump = true;
    [Tooltip("Activa coyote time")] public bool coyoteTimeEnabled = true;
    [Tooltip("Duración de coyote time")] public float coyoteTimeDuration = 0.25f;

    [Header("Ground Check")]
    [Tooltip("Punto de comprobación de suelo")] public Transform groundCheck;
    [Tooltip("Radio de la esfera de comprobación")] public float groundDistance = 0.3f;
    [Tooltip("Capas consideradas como suelo")] public LayerMask groundMask;

    // Implementación de IMovementModule
    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsMoveEnabled { get; set; } = true;

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector2 moveInput;
    private float coyoteTimer;

    public Vector3 Velocity => controller.velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void HandleMovement()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (IsGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f;
            coyoteTimer = coyoteTimeEnabled ? coyoteTimeDuration : 0f;
        }
        else if (coyoteTimeEnabled)
        {
            coyoteTimer -= Time.deltaTime;
        }

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        bool holdingShift = Input.GetKey(KeyCode.LeftShift);
        IsSprinting = canSprint && !holdingShift && moveInput.y > 0.1f && IsGrounded;

        float speed = IsSprinting ? sprintSpeed : walkSpeed;
        if (!IsMoveEnabled) speed = 0f;

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 move = transform.TransformDirection(direction) * speed;
        move = Vector3.ClampMagnitude(move, speed);

        if (IsGrounded || coyoteTimer > 0f)
        {
            if (canJump && Input.GetKeyDown(KeyCode.Space))
                moveDirection.y = jumpSpeed;
            else if (moveDirection.y < 0)
                moveDirection.y = -2f;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        moveDirection.x = move.x;
        moveDirection.z = move.z;
        controller.Move(moveDirection * Time.deltaTime);
    }
}