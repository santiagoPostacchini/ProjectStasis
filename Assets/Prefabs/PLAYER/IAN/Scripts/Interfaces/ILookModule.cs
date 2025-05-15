// Interfaces/ILookModule.cs
public interface ILookModule
{
    //bool IsSliding { get; set; }
    bool IsLookEnabled { get; set; }
    void HandleLook();
}
