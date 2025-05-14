// PlayerLook.cs
using UnityEngine;
[RequireComponent(typeof(PlayerSlide))]
public class PlayerLook : MonoBehaviour
{
    [Header("Look Settings")]
    [Tooltip("Sensibilidad del mouse para la cámara")] public float mouseSensitivity = 25f;
    [Tooltip("Suavidad en la interpolación de rotación")]
    [Range(0f, 200f)] public float snappiness = 100f;
    [Tooltip("Ángulo de tilt aplicado a la cámara durante el slide")]
    public float slideTiltAngle = 5f;

    [Tooltip("Transform de la cámara del jugador")] public Transform playerCamera;
    [HideInInspector] public bool IsSliding;
    [HideInInspector] public bool IsLookEnabled = true;

    private float rotX, rotY;
    private float xVelocity, yVelocity;
    private PlayerSlide slideModule;

    void Awake()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
        slideModule = GetComponent<PlayerSlide>();
    }

    public void HandleLook()
    {
        if (!IsLookEnabled || playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * 10f * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 10f * mouseSensitivity * Time.deltaTime;

        rotX += mouseX;
        rotY -= mouseY;
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        xVelocity = Mathf.Lerp(xVelocity, rotX, snappiness * Time.deltaTime);
        yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

        if (slideModule.IsSliding)
        {
            float targetX = yVelocity - slideTiltAngle;
            playerCamera.localRotation = Quaternion.Lerp(
                playerCamera.localRotation,
                Quaternion.Euler(targetX, 0f, 0f),
                Time.deltaTime * 10f
            );
        }
        else
        {
            playerCamera.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
        }
        transform.rotation = Quaternion.Euler(0f, rotX, 0f);
    }
}
