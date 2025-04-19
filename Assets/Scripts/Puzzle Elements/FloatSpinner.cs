using UnityEngine;

/// <summary>
/// Gira el objeto sobre sus tres ejes y, opcionalmente, lo hace “flotar”.
/// Asigna el script al cubo (o Prefab) y ajusta los valores desde el Inspector.
/// </summary>
[AddComponentMenu("Gameplay/Float Spinner")]
public class FloatSpinner : MonoBehaviour
{
    /* ───────────── ROTACIÓN ───────────── */
    [Header("Rotación (°/segundo)")]
    [Tooltip("Velocidad de rotación alrededor del eje X (Pitch). 0 desactiva la rotación en X.")]
    [SerializeField, Range(-360f, 360f)] private float speedX = 0f;

    [Tooltip("Velocidad de rotación alrededor del eje Y (Yaw). 0 desactiva la rotación en Y.")]
    [SerializeField, Range(-360f, 360f)] private float speedY = 45f;

    [Tooltip("Velocidad de rotación alrededor del eje Z (Roll). 0 desactiva la rotación en Z.")]
    [SerializeField, Range(-360f, 360f)] private float speedZ = 0f;

    /* ───────────── FLOTACIÓN ───────────── */
    [Header("Flotación opcional")]
    [Tooltip("Altura máxima de la oscilación vertical. 0 desactiva la flotación.")]
    [SerializeField, Min(0f)] private float bobAmplitude = 0.1f;

    [Tooltip("Frecuencia de la oscilación vertical (ciclos por segundo).")]
    [SerializeField, Range(0.1f, 5f)] private float bobFrequency = 1f;

    /* ───────────── PRIVADOS ───────────── */
    private Vector3 _startPos;

    /* ───────────── CICLO DE VIDA ───────────── */
    private void Start()
    {
        _startPos = transform.localPosition;
    }

    private void Update()
    {
        // Rotación continua en 3 ejes
        Vector3 rotationStep = new Vector3(speedX, speedY, speedZ) * Time.deltaTime;
        transform.Rotate(rotationStep, Space.Self);

        // Flotación vertical opcional
        if (bobAmplitude > 0f)
        {
            float offsetY = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
            Vector3 newPos = _startPos;
            newPos.y += offsetY;
            transform.localPosition = newPos;
        }
    }
}
