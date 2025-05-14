// PlayerSlide.cs
using UnityEngine;
[RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
public class PlayerSlide : MonoBehaviour
{
    [Header("Slide Settings")]
    [Tooltip("Velocidad durante el slide")] public float slideSpeed = 9f;
    [Tooltip("Duración del slide en segundos")] public float slideDuration = 0.7f;
    [Tooltip("Permite deslizamiento")] public bool canSlide = true;

    [HideInInspector] public bool IsSliding;

    private float slideTimer;
    private Vector3 slideDirection;
    private CharacterController controller;
    private PlayerMovement movementModule;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        movementModule = GetComponent<PlayerMovement>();
    }

    public void HandleSlide()
    {
        if (IsSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f || !movementModule.IsGrounded)
                IsSliding = false;

            float progress = slideTimer / slideDuration;
            float currentSpeed = slideSpeed * Mathf.Lerp(0.5f, 1f, progress * progress);
            controller.Move(slideDirection * currentSpeed * Time.deltaTime);
            return;
        }

        if (canSlide && movementModule.IsSprinting && Input.GetKeyDown(KeyCode.LeftControl) && movementModule.IsGrounded)
        {
            IsSliding = true;
            slideTimer = slideDuration;
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            slideDirection = input.magnitude > 0.1f
                ? (transform.right * input.x + transform.forward * input.y).normalized
                : transform.forward;
        }
    }
}