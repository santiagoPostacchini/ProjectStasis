using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Control de Rotación")]
    [Tooltip("Activa o desactiva la rotación.")]
    public bool canRotate = true;
    [Tooltip("Activa el modo errático (random suave cada intervalo).")]
    public bool erratic = false;

    [Header("Velocidad Fija (si no es errática)")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);

    [Header("Rango Velocidad Errática")]
    [Tooltip("Valor mínimo para cada eje cuando erratic = true")]
    public Vector3 minSpeed = new Vector3(-100f, -100f, -100f);
    [Tooltip("Valor máximo para cada eje cuando erratic = true")]
    public Vector3 maxSpeed = new Vector3(100f, 100f, 100f);

    [Header("Configuración Errática")]
    [Tooltip("Segundos que tarda en generar y completar la transición a la siguiente velocidad")]
    public float changeInterval = 1f;

    // estado interno
    private Vector3 _currentTarget;
    private Vector3 _nextTarget;
    private float _timer;

    void Start()
    {
        // inicializo dos velocidades aleatorias
        _currentTarget = GenerateRandomSpeed();
        _nextTarget = GenerateRandomSpeed();
        _timer = 0f;
    }

    void Update()
    {
        if (!canRotate) return;

        Vector3 speed = erratic
            ? GetErraticSpeed()
            : rotationSpeed;

        transform.Rotate(speed * Time.deltaTime, Space.Self);
    }

    private Vector3 GetErraticSpeed()
    {
        // avanzo el timer
        _timer += Time.deltaTime;
        float t = _timer / changeInterval;

        // si superé el intervalo, paso al siguiente target y genero uno nuevo
        if (_timer >= changeInterval)
        {
            _timer -= changeInterval;
            _currentTarget = _nextTarget;
            _nextTarget = GenerateRandomSpeed();
            t = _timer / changeInterval;
        }

        // interpolo suavemente entre ambos vectores
        return Vector3.Lerp(_currentTarget, _nextTarget, t);
    }

    private Vector3 GenerateRandomSpeed()
    {
        return new Vector3(
            Random.Range(minSpeed.x, maxSpeed.x),
            Random.Range(minSpeed.y, maxSpeed.y),
            Random.Range(minSpeed.z, maxSpeed.z)
        );
    }
}
