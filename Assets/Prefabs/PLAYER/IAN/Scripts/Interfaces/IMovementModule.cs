// Interfaces/IMovementModule.cs
public interface IMovementModule
{
    bool IsGrounded { get; }
    bool IsSprinting { get; }
    bool IsMoveEnabled { get; set; }
    void HandleMovement();
}