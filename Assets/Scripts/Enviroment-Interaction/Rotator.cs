using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Velocidad de Rotación (grados/segundo)")]
    [Tooltip("Define la velocidad de rotación en cada eje (X, Y, Z).")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);
    public bool canRotate;
    void Update()
    {
        // Rota el objeto en el espacio local (Space.Self) multiplicando por Time.deltaTime
        Rotate();
    }
    public void Rotate()
    {
        if (!canRotate) return;
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
