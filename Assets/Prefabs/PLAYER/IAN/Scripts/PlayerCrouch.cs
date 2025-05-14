// PlayerCrouch.cs
using UnityEngine;
[RequireComponent(typeof(CharacterController), typeof(PlayerSlide))]
public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float crouchCameraHeight = 0.5f;
    public bool canCrouch = true;
    public float postSlideCrouchTime = 0.1f;
    public LayerMask groundMask;

    public Transform cameraParent;

    [HideInInspector] public bool IsCrouching;

    private CharacterController controller;
    private PlayerSlide slideModule;
    private float originalHeight;
    private float originalCamY;
    private float postSlideTimer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        slideModule = GetComponent<PlayerSlide>();
        originalHeight = controller.height;
        originalCamY = cameraParent.localPosition.y;
    }

    public void HandleCrouch()
    {
        if (slideModule.IsSliding)
        {
            postSlideTimer = postSlideCrouchTime;
        }

        if (postSlideTimer > 0f)
        {
            postSlideTimer -= Time.deltaTime;
            IsCrouching = canCrouch;
        }
        else
        {
            bool wants = canCrouch && Input.GetKey(KeyCode.LeftControl) && !slideModule.IsSliding;
            Vector3 p1 = transform.position + controller.center - Vector3.up * (controller.height * 0.5f);
            Vector3 p2 = p1 + Vector3.up * (controller.height * 0.6f);
            float radius = controller.radius * 0.95f;
            float castDist = slideModule.IsSliding ? originalHeight + 0.2f : originalHeight - crouchHeight + 0.2f;
            bool ceiling = Physics.CapsuleCast(p1, p2, radius, Vector3.up, castDist, groundMask);
            IsCrouching = canCrouch && (wants || (ceiling && !slideModule.IsSliding));
        }

        float targetH = (IsCrouching || slideModule.IsSliding) ? crouchHeight : originalHeight;
        controller.height = Mathf.Lerp(controller.height, targetH, Time.deltaTime * 15f);
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);

        float targetY = (IsCrouching || slideModule.IsSliding) ? crouchCameraHeight : originalCamY;
        cameraParent.localPosition = new Vector3(
            cameraParent.localPosition.x,
            Mathf.Lerp(cameraParent.localPosition.y, targetY, Time.deltaTime * 15f),
            cameraParent.localPosition.z
        );
    }
}
