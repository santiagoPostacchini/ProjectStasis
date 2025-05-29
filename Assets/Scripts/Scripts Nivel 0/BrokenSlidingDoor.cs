using System.Collections;
using UnityEngine;
using Events;

[RequireComponent(typeof(Collider))]
public class BrokenSlidingDoorController : MonoBehaviour
{
    [Header("Referencias de hijos")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform[] gears;

    [Header("Engranajes")]
    [Tooltip("Ángulo total que giran los engranajes")]
    public float gearRotationAngle = 180f;
    [Tooltip("Duración en segundos del giro de los engranajes")]
    public float gearRotateDuration = 0.5f;
    public string gearSoundName = "GearTurn";

    [Header("Movimiento roto")]
    [Tooltip("Distancia que recorre la hoja rota")]
    public float slideDistance = 0.5f;
    [Tooltip("Duración del intento de apertura/cierre")]
    public float slideDuration = 1.5f;
    [Tooltip("Cantidad de vibración aleatoria")]
    public float jitterAmount = 0.01f;
    [Tooltip("Velocidad de vibración")]
    public float jitterSpeed = 25f;

    [Header("Engranajes rotos")]
    [Tooltip("Ángulo de giro errático")]
    public float gearJitterAngle = 10f;
    [Tooltip("Velocidad de giro errático")]
    public float gearJitterSpeed = 2f;

    [Header("Puerta SFX")]
    public string openSoundName = "Door.OPEN";
    public string closeSoundName = "Door.CLOSE";

    [Header("Control de estado")]
    [Tooltip("Marca para abrir/cerrar la puerta")]
    public bool isOpen = false;
    private bool lastState = false;

    [Header("Partículas (opcional)")]
    [SerializeField] private ParticleSystem[] particlesDoor = new ParticleSystem[3];

    // Posiciones iniciales
    private Vector3 rightClosedPos, rightOpenPos;

    private void Start()
    {
        rightClosedPos = rightDoor.localPosition;
        rightOpenPos = rightClosedPos + Vector3.left * slideDistance;
        lastState = isOpen;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (isOpen != lastState)
        {
            StopAllCoroutines();
            StartCoroutine(RunDoorSequence(isOpen));
            lastState = isOpen;
        }

        // Siempre giran erráticamente
        //RotateGearsBroken();
    }


    private IEnumerator RunDoorSequence(bool opening)
    {
        PlayParticles();

        if (opening)
        {
            // Girar engranajes en orden, uno por uno
            foreach (var gear in gears)
            {
                yield return RotateGear(gear, true);
            }

            // Una vez que todos giraron, reproducir sonido de abrir y mover la puerta
            EventManager.TriggerEvent(openSoundName, gameObject);
            yield return SlideDoorsBroken();
        }
        else
        {
            // Primero reproducir sonido de cerrar y luego mover la puerta
            EventManager.TriggerEvent(closeSoundName, gameObject);
            yield return SlideDoorsBroken();
        }
    }

    public void OpenDoor()
    {
        if (!isOpen) isOpen = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (!isOpen) OpenDoor();
    }

    private IEnumerator SlideDoorsBroken()
    {
        float elapsed = 0f;
        Vector3 start = rightDoor.localPosition;
        Vector3 end = isOpen ? rightOpenPos : rightClosedPos;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            Vector3 basePos = Vector3.LerpUnclamped(start, end, t);
            float jitter = Mathf.Sin(Time.time * jitterSpeed) * jitterAmount;

            rightDoor.localPosition = basePos + Vector3.up * jitter;
            yield return null;
        }

        rightDoor.localPosition = end;
    }

    private IEnumerator RotateGear(Transform gear, bool opening)
    {
        EventManager.TriggerEvent(gearSoundName, gameObject);

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

    private void RotateGearsBroken()
    {
        foreach (var gear in gears)
        {
            float angle = Mathf.Sin(Time.time * gearJitterSpeed) * gearJitterAngle;
            Vector3 euler = gear.localEulerAngles;
            gear.localEulerAngles = new Vector3(euler.x, euler.y, angle);
        }
    }

    private void PlayParticles()
    {
        foreach (var p in particlesDoor)
        {
            if (p != null) p.Play();
        }
    }
}