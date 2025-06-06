using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Velocidad de Rotación (grados/segundo)")]
    [Tooltip("Define la velocidad de rotación en cada eje (X, Y, Z).")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);

    void Update()
    {
        // Rota el objeto en el espacio local (Space.Self) multiplicando por Time.deltaTime
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
