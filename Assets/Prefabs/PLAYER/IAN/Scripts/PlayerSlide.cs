// PlayerSlide.cs
using UnityEngine;
[RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
public class PlayerSlide : MonoBehaviour, ISlideModule
{
    [Header("Slide Settings")]
    [Tooltip("Velocidad durante el slide")] public float slideSpeed = 9f;
    [Tooltip("Duraci�n del slide en segundos")] public float slideDuration = 0.7f;
    [Tooltip("Permite deslizamiento")] public bool canSlide = true;

    public bool IsSliding { get; private set; }

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
            Vector2 inp = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            slideDirection = inp.magnitude > 0.1f
                ? (transform.right * inp.x + transform.forward * inp.y).normalized
                : transform.forward;
        }
    }
}