// MovementSettings.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Header("Look")]
    public float MouseSensitivity = 25f;
    public float Snappiness = 100f;

    [Header("Speeds")]
    public float WalkSpeed = 3f;
    public float SprintSpeed = 5f;
    public float CrouchSpeed = 1.5f;
    public float SlideSpeed = 9f;

    [Header("Slide")]
    public float SlideDuration = 0.7f;
    public float SlideFovBoost = 5f;
    public float SlideTiltAngle = 5f;

    [Header("Jump & Gravity")]
    public float JumpHeight = 1.5f;
    public float Gravity = -9.81f;
    [Tooltip("Control aéreo, de 0 (ninguno) a 1 (igual que suelo)")]
    public float AirControl = 0.3f;
    public bool CoyoteTimeEnabled = true;
    public float CoyoteTimeDuration = 0.2f;

    [Header("FOV")]
    public float NormalFov = 60f;
    public float SprintFov = 70f;
    public float FovChangeSpeed = 5f;

    [Header("Head Bob")]
    public float BobAmount = 0.1f;
    public float BobSpeed = 2f;
    public float CrouchCamHeight = 0.5f;

    [Header("Ground Check")]
    public float GroundDistance = 0.2f;
}
