// SlidingDoorController.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlidingDoorController : MonoBehaviour
{
    [Header("Referencias de hijos")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform[] gears;

    [Header("Deslizamiento")]
    [Tooltip("Distancia en unidades que recorre cada hoja")]
    public float slideDistance = 2f;
    [Tooltip("Duración en segundos del deslizamiento")]
    public float slideDuration = 1f;

    [Header("Engranajes")]
    [Tooltip("Ángulo total que giran los engranajes")]
    public float gearRotationAngle = 180f;
    [Tooltip("Duración en segundos del giro de los engranajes")]
    public float gearRotateDuration = 0.5f;
    public string gearSoundName = "GearTurn";

    [Header("Puerta SFX")]
    public string openSoundName = "Door.OPEN";
    public string closeSoundName = "Door.CLOSE";

    [Header("Control de estado")]
    [Tooltip("Marca para abrir/cerrar la puerta")]
    public bool isOpen = false;
    private bool lastState = false;

    [Header("Autocierre por Trigger")]
    public bool autoClose = true;
    [Tooltip("Retraso antes de cerrar tras detectar al jugador")]
    public float autoCloseDelay = 1f;

    [Header("Cierre temporizado")]
    [Tooltip("Si true, la puerta se cerrará automáticamente tras abrirse")]
    public bool hasTimedClose = false;
    [Tooltip("Tiempo en segundos que la puerta permanece abierta antes de cerrarse")]
    public float timedCloseDelay = 5f;

    // Posiciones iniciales
    private Vector3 leftClosedPos, rightClosedPos;
    // Para cancelar cierres temporizados antiguos
    private Coroutine timedCloseCoroutine;

    private void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;
        lastState = isOpen;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (isOpen != lastState)
        {
            StopAllCoroutines();
            // Cancelar cualquier cierre programado anterior
            if (timedCloseCoroutine != null)
                StopCoroutine(timedCloseCoroutine);

            StartCoroutine(RunDoorSequence(isOpen));
            lastState = isOpen;
        }
    }

    private IEnumerator RunDoorSequence(bool opening)
    {
        if (opening)
        {
            // 1) Giro de engranajes antes de abrir
            foreach (var gear in gears)
            {
                AudioManager.Instance.PlaySfx(gearSoundName);
                yield return RotateGear(gear, true);
            }

            // 2) Sonido de apertura
            AudioManager.Instance.PlaySfx(openSoundName);

            // 3) Deslizamiento abriendo
            yield return SlideDoors(true);

            // 4) Si tiene cierre temporizado, arrancamos rutina
            if (hasTimedClose)
                timedCloseCoroutine = StartCoroutine(TimedCloseRoutine());
        }
        else
        {
            // 1) Sonido de cierre
            AudioManager.Instance.PlaySfx(closeSoundName);

            // 2) Deslizamiento cerrando
            yield return SlideDoors(false);

            // 3) Giro de engranajes al final del cierre
            foreach (var gear in gears)
            {
                AudioManager.Instance.PlaySfx(gearSoundName);
                yield return RotateGear(gear, false);
            }
        }
    }

    private IEnumerator TimedCloseRoutine()
    {
        yield return new WaitForSeconds(timedCloseDelay);
        isOpen = false;
    }

    private IEnumerator RotateGear(Transform gear, bool opening)
    {
        float elapsed = 0f;
        float from = gear.localEulerAngles.z;
        float to = opening ? from + gearRotationAngle : from - gearRotationAngle;

        while (elapsed < gearRotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / gearRotateDuration);
            float angle = Mathf.LerpUnclamped(from, to, t);
            var e = gear.localEulerAngles;
            gear.localEulerAngles = new Vector3(e.x, e.y, angle);
            yield return null;
        }

        var final = gear.localEulerAngles;
        gear.localEulerAngles = new Vector3(final.x, final.y, to);
    }

    private IEnumerator SlideDoors(bool opening)
    {
        Vector3 leftStart = leftDoor.localPosition;
        Vector3 rightStart = rightDoor.localPosition;

        Vector3 leftEnd = opening ? leftClosedPos + Vector3.right * slideDistance : leftClosedPos;
        Vector3 rightEnd = opening ? rightClosedPos + Vector3.left * slideDistance : rightClosedPos;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            leftDoor.localPosition = Vector3.LerpUnclamped(leftStart, leftEnd, t);
            rightDoor.localPosition = Vector3.LerpUnclamped(rightStart, rightEnd, t);
            yield return null;
        }

        leftDoor.localPosition = leftEnd;
        rightDoor.localPosition = rightEnd;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player"))
    //        return;

    //    if (!isOpen)
    //        isOpen = true;
    //    else if (autoClose)
    //        StartCoroutine(AutoCloseRoutine());
    //}

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        isOpen = false;
    }
}
