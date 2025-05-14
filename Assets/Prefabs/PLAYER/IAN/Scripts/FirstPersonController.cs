// FirstPersonController.cs
using UnityEngine;

[RequireComponent(typeof(PlayerLook), typeof(PlayerMovement), typeof(PlayerCrouch))]
[RequireComponent(typeof(PlayerHeadBob))]
public class FirstPersonController : MonoBehaviour
{
    private IMovementModule movement;
    private ILookModule look;
    private ICrouchModule crouch;
    private ITiltModule tilt;

    void Awake()
    {
        movement = GetComponent<IMovementModule>();
        look = GetComponent<ILookModule>();
        crouch = GetComponent<ICrouchModule>();
        tilt = GetComponent<ITiltModule>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        movement.HandleMovement();
        crouch.HandleCrouch();

        look.HandleLook();
        tilt.HandleTilt();
    }

    public void SetControl(bool enabled)
    {
        look.IsLookEnabled = enabled;
        movement.IsMoveEnabled = enabled;
    }

    public void SetCursorVisibility(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}