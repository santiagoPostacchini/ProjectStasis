// PlayerLook.cs
using UnityEngine;
public class PlayerLook : MonoBehaviour, ILookModule
{
    [Header("Look Settings")]
    [Tooltip("Sensibilidad del mouse para la cámara")] public float mouseSensitivity = 25f;
    [Tooltip("Suavidad en la interpolación de rotación")]
    [Range(0f, 200f)] public float snappiness = 100f;

    [Tooltip("Transform de la cámara del jugador")] public Transform playerCamera;

    [HideInInspector]
    public bool IsLookEnabled { get; set; } = true;

    private float rotX, rotY;
    private float xVelocity, yVelocity;

    void Awake()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
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

        playerCamera.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, rotX, 0f);
    }
}