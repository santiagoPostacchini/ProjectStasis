/// <summary>
/// PlayerHeadBob
/// Aplica únicamente el tilt horizontal a la cámara, sin head bobbing vertical.
/// </summary>
// PlayerHeadBob.cs
using UnityEngine;
[RequireComponent(typeof(PlayerMovement), typeof(PlayerSlide), typeof(PlayerCrouch))]
public class PlayerHeadBob : MonoBehaviour
{
    [Header("Tilt Settings")]
    [Tooltip("Transform que contiene la cámara para aplicar tilt horizontal")] public Transform cameraParent;
    [Tooltip("Intensidad de tilt lateral al moverse horizontalmente")] public float lateralTiltAmount = 2f;
    [Tooltip("Velocidad de retorno del tilt al estado neutral")] public float recoilReturnSpeed = 8f;

    private PlayerMovement movementModule;
    private PlayerSlide slideModule;
    private PlayerCrouch crouchModule;
    private Vector3 recoil;

    void Awake()
    {
        movementModule = GetComponent<PlayerMovement>();
        slideModule = GetComponent<PlayerSlide>();
        crouchModule = GetComponent<PlayerCrouch>();
    }

    /// <summary>
    /// Ejecuta la lógica de tilt lateral cada frame. Mantiene la posición de cámara intacta.
    /// </summary>
    public void HandleHeadBob()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool sliding = slideModule.IsSliding;
        bool crouch = crouchModule.IsCrouching;

        if (sliding || crouch)
        {
            recoil = Vector3.zero;
        }
        else
        {
            recoil.z = input.x * -lateralTiltAmount;
        }

        cameraParent.localRotation = Quaternion.RotateTowards(
            cameraParent.localRotation,
            Quaternion.Euler(recoil),
            recoilReturnSpeed * Time.deltaTime
        );
    }
}
