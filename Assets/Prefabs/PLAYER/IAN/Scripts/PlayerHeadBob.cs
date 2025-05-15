// PlayerHeadBob.cs
using UnityEngine;
public class PlayerHeadBob : MonoBehaviour, ITiltModule
{
    [Header("Tilt Settings")]
    [Tooltip("Transform que contiene la cámara para aplicar tilt horizontal")] public Transform cameraParent;
    [Tooltip("Intensidad de tilt lateral al moverse horizontalmente")] public float lateralTiltAmount = 2f;
    [Tooltip("Velocidad de retorno del tilt al estado neutral")] public float recoilReturnSpeed = 8f;

    private Vector3 recoil;

    void Awake()
    {
        if (cameraParent == null && Camera.main) cameraParent = Camera.main.transform;
    }

    public void HandleTilt()
    {
        float inputX = Input.GetAxis("Horizontal");
        recoil.z = inputX * -lateralTiltAmount;
        cameraParent.localRotation = Quaternion.RotateTowards(
            cameraParent.localRotation,
            Quaternion.Euler(recoil),
            recoilReturnSpeed * Time.deltaTime);
    }
}
