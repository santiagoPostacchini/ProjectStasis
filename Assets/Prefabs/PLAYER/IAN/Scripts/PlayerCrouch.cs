// PlayerCrouch.cs
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerCrouch : MonoBehaviour, ICrouchModule
{
    [Header("Crouch Settings")]
    [Tooltip("Altura al agacharse")] public float crouchHeight = 1f;
    [Tooltip("Posición de la cámara cuando está agachado")] public float crouchCameraHeight = 0.5f;
    [Tooltip("Permite agacharse y levantarse")] public bool canCrouch = true;
    [Tooltip("Capas para detección de techo")] public LayerMask groundMask;

    [Tooltip("Transform que contiene la cámara para ajustar posición vertical")] public Transform cameraParent;

    public bool IsCrouching { get; private set; }

    private CharacterController controller;
    private float originalHeight;
    private float originalCamY;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraParent == null && Camera.main) cameraParent = Camera.main.transform;
        originalHeight = controller.height;
        originalCamY = cameraParent ? cameraParent.localPosition.y : 0f;
    }

    public void HandleCrouch()
    {
        bool wants = canCrouch && Input.GetKey(KeyCode.LeftControl);
        Vector3 p1 = transform.position + controller.center - Vector3.up * (controller.height * 0.5f);
        Vector3 p2 = p1 + Vector3.up * (controller.height * 0.6f);
        float radius = controller.radius * 0.95f;
        float castDist = originalHeight - crouchHeight + 0.2f;
        bool ceiling = Physics.CapsuleCast(p1, p2, radius, Vector3.up, castDist, groundMask);

        IsCrouching = wants && !ceiling;
        float targetH = IsCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetH, Time.deltaTime * 15f);
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);

        if (cameraParent)
        {
            float targetY = IsCrouching ? crouchCameraHeight : originalCamY;
            cameraParent.localPosition = new Vector3(
                cameraParent.localPosition.x,
                Mathf.Lerp(cameraParent.localPosition.y, targetY, Time.deltaTime * 15f),
                cameraParent.localPosition.z);
        }
    }
}