// SlidingDoorController.cs
using System.Collections;
using Events;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
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
        public float gearRotateDuration = .5f;
        [Tooltip("Nombre del evento de sonido para el giro de engranajes")]
        public string gearEventName = "GearTurn";

        [Header("Puerta SFX (eventos)")]
        [Tooltip("Evento que se dispara al abrir")]
        public string openEventName = "Door.OPEN";
        [Tooltip("Evento que se dispara al cerrar")]
        public string closeEventName = "Door.CLOSE";

        [Header("Control de estado")]
        [Tooltip("Marca para abrir/cerrar la puerta")]
        public bool isOpen;
        private bool _lastState;

        [Header("Autocierre por Trigger")]
        public bool autoClose = true;
        [Tooltip("Retraso antes de cerrar tras detectar al jugador")]
        public float autoCloseDelay = 1f;

        [Header("Cierre temporizado")]
        [Tooltip("Si true, la puerta se cerrará tras abrirse")]
        public bool hasTimedClose;
        [Tooltip("Tiempo en segundos que la puerta permanece abierta")]
        public float timedCloseDelay = 5f;

        [Header("Partículas")]
        [Tooltip("Particle Systems que se disparan en el slide")]
        public ParticleSystem[] particlesDoor = new ParticleSystem[3];

        // Posiciones iniciales
        private Vector3 _leftClosedPos, _rightClosedPos;
        private Coroutine _timedCloseCoroutine;

        private void Start()
        {
            _leftClosedPos  = leftDoor.localPosition;
            _rightClosedPos = rightDoor.localPosition;
            _lastState      = isOpen;

            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void Update()
        {
            if (isOpen == _lastState) return;

            StopAllCoroutines();
            if (_timedCloseCoroutine != null)
                StopCoroutine(_timedCloseCoroutine);

            StartCoroutine(RunDoorSequence(isOpen));
            _lastState = isOpen;
        }

        private IEnumerator RunDoorSequence(bool opening)
        {
            if (opening)
            {
                // 1) Giro de engranajes antes de abrir
                foreach (var gear in gears)
                {
                    EventManager.TriggerEvent(gearEventName, gameObject);
                    yield return RotateGear(gear, true);
                }

                // 2) Evento de apertura
                EventManager.TriggerEvent(openEventName, gameObject);

                // 3) Deslizamiento abriendo
                yield return SlideDoors(true);

                // 4) Partículas en el slide
                foreach (var ps in particlesDoor)
                    ps.Play();

                // 5) Cierre temporizado si toca
                if (hasTimedClose)
                    _timedCloseCoroutine = StartCoroutine(TimedCloseRoutine());
            }
            else
            {
                // 1) Evento de cierre
                EventManager.TriggerEvent(closeEventName, gameObject);

                // 2) Deslizamiento cerrando
                yield return SlideDoors(false);

                // 3) Giro de engranajes al final del cierre
                foreach (var gear in gears)
                {
                    EventManager.TriggerEvent(gearEventName, gameObject);
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
            float to   = opening
                ? from + gearRotationAngle
                : from - gearRotationAngle;

            while (elapsed < gearRotateDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / gearRotateDuration);
                var e = gear.localEulerAngles;
                e.z = Mathf.LerpUnclamped(from, to, t);
                gear.localEulerAngles = e;
                yield return null;
            }

            var final = gear.localEulerAngles;
            final.z = to;
            gear.localEulerAngles = final;
        }

        private IEnumerator SlideDoors(bool opening)
        {
            Vector3 leftStart  = leftDoor.localPosition;
            Vector3 rightStart = rightDoor.localPosition;

            Vector3 leftEnd  = opening
                ? _leftClosedPos  + Vector3.right * slideDistance
                : _leftClosedPos;
            Vector3 rightEnd = opening
                ? _rightClosedPos + Vector3.left  * slideDistance
                : _rightClosedPos;

            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
                leftDoor.localPosition  = Vector3.LerpUnclamped(leftStart,  leftEnd,  t);
                rightDoor.localPosition = Vector3.LerpUnclamped(rightStart, rightEnd, t);
                yield return null;
            }

            leftDoor.localPosition  = leftEnd;
            rightDoor.localPosition = rightEnd;
        }

        private IEnumerator AutoCloseRoutine()
        {
            yield return new WaitForSeconds(autoCloseDelay);
            isOpen = false;
        }
    }
}
