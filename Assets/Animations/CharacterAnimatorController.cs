using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour
{
    [Header("References")]
    public PlayerController2 playerController; // Asignalo desde el Inspector
    public Rigidbody playerRb;                // Asignalo desde el Inspector
    public GroundDetector groundDetector;     // Asignalo desde el Inspector

    private Animator animator;

    [Header("Animation Settings")]
    public float speedSmoothing = 15f; // Cuanto mayor, más rápido reacciona

    private float currentSpeed;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Verificaciones opcionales
        if (!playerController) playerController = GetComponentInParent<PlayerController2>();
        if (!playerRb) playerRb = GetComponentInParent<Rigidbody>();
        if (!groundDetector) groundDetector = GetComponentInParent<GroundDetector>();
    }

    void Update()
    {
        if (playerController == null || playerRb == null || groundDetector == null) return;

        // Cálculo de velocidad horizontal (sin eje Y)
        Vector3 flatVelocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
        float targetSpeed = flatVelocity.magnitude;

        // Suavizado (más responsivo con smoothing alto)
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedSmoothing);

        // Aplicar al Animator
        animator.SetFloat("speed", currentSpeed);

        // Piso
        animator.SetBool("isGrounded", groundDetector.isGrounded);

        // Trigger de salto si estás subiendo y no estás en el suelo
        if (!groundDetector.isGrounded && playerRb.velocity.y > 0.1f)
        {
            animator.SetTrigger("isJumping");
        }
    }
}
