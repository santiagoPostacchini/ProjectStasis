using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BrokenSlidingDoorController : MonoBehaviour
{
    [Header("Referencias de hijos")]
    public Transform leftDoor;
    public Transform rightDoor;
    public Transform[] gears;

    [Header("Movimiento roto")]
    [Tooltip("Distancia que recorre la hoja rota")]
    public float slideDistance = 0.5f;
    [Tooltip("Duraci�n del intento de apertura/cierre")]
    public float slideDuration = 1.5f;
    [Tooltip("Cantidad de vibraci�n aleatoria")]
    public float jitterAmount = 0.01f;
    [Tooltip("Velocidad de vibraci�n")]
    public float jitterSpeed = 25f;

    [Header("Engranajes rotos")]
    [Tooltip("�ngulo de giro err�tico")]
    public float gearJitterAngle = 10f;
    [Tooltip("Velocidad de giro err�tico")]
    public float gearJitterSpeed = 2f;
    public string gearSoundName = "GearTurn";

    [Header("Puerta SFX")]
    public string openSoundName = "Door.OPEN";
    public string closeSoundName = "Door.CLOSE";

    [Header("Control de estado")]
    [Tooltip("Marca para abrir/cerrar la puerta")]
    public bool isOpen = false;
    private bool lastState = false;

    [Header("Part�culas (opcional)")]
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

        // Siempre giran err�ticamente
        RotateGearsBroken();
    }

    private IEnumerator RunDoorSequence(bool opening)
    {
        PlayParticles();

        if (opening)
        {
            // No hay sonido real, solo giro falso
            foreach (var gear in gears)
            {
                AudioManager.Instance.PlaySfx(gearSoundName);
                yield return new WaitForSeconds(0.1f);
            }
            AudioManager.Instance.PlaySfx(openSoundName);
            yield return SlideDoorsBroken();
        }
        else
        {
            AudioManager.Instance.PlaySfx(closeSoundName);
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
        if (!isOpen) isOpen = true;
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