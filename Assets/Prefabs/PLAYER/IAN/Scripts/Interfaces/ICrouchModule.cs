// Interfaces/ICrouchModule.cs
public interface ICrouchModule
{
    bool IsCrouching { get; }
    void HandleCrouch();
}