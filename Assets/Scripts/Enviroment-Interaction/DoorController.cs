using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public Transform doorTransform;              // El Transform de la puerta (puede ser un child)
    public Vector3 closedEulerAngles;            // Rotación en estado cerrado (en inspector)
    public Vector3 openEulerAngles;              // Rotación en estado abierto (en inspector)

    [Header("Animación")]
    [Tooltip("Duración en segundos de la animación de apertura/cierre")]
    public float animationDuration = 1f;
    private bool isAnimating = false;

    [Header("Control de Estado")]
    [Tooltip("Marca para abrir/cerrar la puerta")]
    public bool isOpen = false;
    private bool lastState = false;

    [Header("Autocierre")]
    [Tooltip("Si true, la puerta se cerrará automáticamente tras que el jugador pase")]
    public bool autoClose = true;
    [Tooltip("Tiempo en segundos tras detectar al jugador para cerrar")]
    public float closeDelay = 1f;

    [Header("Sonidos")]
    public string openSoundName = "DoorOpen";
    public string closeSoundName = "DoorClose";

    private void Start()
    {
        // Inicializamos la rotación al estado declarado
        doorTransform.localEulerAngles = closedEulerAngles;
        lastState = isOpen;
    }

    private void Update()
    {
        // Detectamos cambio de estado de isOpen en el inspector o por otro script
        if (isOpen != lastState && !isAnimating)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateDoor(isOpen));
            lastState = isOpen;
        }
    }

    private IEnumerator AnimateDoor(bool opening)
    {
        isAnimating = true;

        // Sonido
        if (opening)
            AudioManager.Instance.PlayAmbient(openSoundName);
        else
            AudioManager.Instance.PlayAmbient(closeSoundName);

        // Interpolación suave de ángulos
        Vector3 startRot = doorTransform.localEulerAngles;
        Vector3 endRot = opening ? openEulerAngles : closedEulerAngles;
        float elapsed = 0f;

        // Aseguramos que use la ruta angular más corta
        startRot = NormalizeEuler(startRot);
        endRot = NormalizeEuler(endRot);

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animationDuration);
            doorTransform.localEulerAngles = Vector3.LerpUnclamped(startRot, endRot, t);
            yield return null;
        }

        doorTransform.localEulerAngles = endRot;
        isAnimating = false;
    }

    // Normaliza cada componente a [-180,180] para evitar vueltas extra
    private Vector3 NormalizeEuler(Vector3 e)
    {
        e.x = Mathf.DeltaAngle(0, e.x);
        e.y = Mathf.DeltaAngle(0, e.y);
        e.z = Mathf.DeltaAngle(0, e.z);
        return e;
    }

    // Si la puerta tiene un Collider marcado como Trigger y el jugador entra...
    private void OnTriggerEnter(Collider other)
    {
        if (!autoClose) return;

        // Ajusta el tag según tu jugador
        if (other.CompareTag("Player") && isOpen && !isAnimating)
        {
            // Inicia autocierre tras el delay
            StartCoroutine(AutoCloseRoutine());
        }
    }

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(closeDelay);
        isOpen = false;
    }
}
