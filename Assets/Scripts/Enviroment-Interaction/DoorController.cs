using System.Collections;
using Events;
using UnityEngine;

// Asegúrate de que exista tu EventManager en el proyecto (sin namespace o importándolo si lo tienes en uno).
namespace Enviroment_Interaction
{
    [RequireComponent(typeof(Collider))]
    public class DoorController : MonoBehaviour
    {
        [Header("Configuración de Rotación")]
        public Transform doorTransform;
        public Vector3 closedEulerAngles;
        public Vector3 openEulerAngles;

        [Header("Animación")]
        [Tooltip("Duración en segundos de la animación de apertura/cierre")]
        public float animationDuration = 1f;
        private bool _isAnimating;

        [Header("Control de Estado")]
        [Tooltip("Marca para abrir/cerrar la puerta")]
        public bool isOpen;
        private bool _lastState;

        [Header("Autocierre")]
        [Tooltip("Si true, la puerta se cerrará automáticamente tras que el jugador pase")]
        public bool autoClose = true;
        [Tooltip("Tiempo en segundos tras detectar al jugador para cerrar")]
        public float closeDelay = 1f;

        [Header("Eventos de Sonido")]
        [Tooltip("Nombre del evento para el sonido de apertura")]
        public string openEventName = "DoorOpen";
        [Tooltip("Nombre del evento para el sonido de cierre")]
        public string closeEventName = "DoorClose";

        private void Start()
        {
            // Inicializamos la rotación al estado declarado
            doorTransform.localEulerAngles = closedEulerAngles;
            _lastState = isOpen;
        }

        private void Update()
        {
            // Detectamos cambio de estado de isOpen
            if (isOpen != _lastState && !_isAnimating)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateDoor(isOpen));
                _lastState = isOpen;
            }
        }

        private IEnumerator AnimateDoor(bool opening)
        {
            _isAnimating = true;

            // Disparamos el evento correspondiente
            string evt = opening ? openEventName : closeEventName;
            EventManager.TriggerEvent(evt, gameObject);

            // Interpolación suave de ángulos
            Vector3 startRot = NormalizeEuler(doorTransform.localEulerAngles);
            Vector3 endRot   = NormalizeEuler(opening ? openEulerAngles : closedEulerAngles);
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / animationDuration);
                doorTransform.localEulerAngles = Vector3.LerpUnclamped(startRot, endRot, t);
                yield return null;
            }

            doorTransform.localEulerAngles = endRot;
            _isAnimating = false;
        }

        private Vector3 NormalizeEuler(Vector3 e)
        {
            e.x = Mathf.DeltaAngle(0, e.x);
            e.y = Mathf.DeltaAngle(0, e.y);
            e.z = Mathf.DeltaAngle(0, e.z);
            return e;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!autoClose) return;
            if (other.CompareTag("Player") && isOpen && !_isAnimating)
                StartCoroutine(AutoCloseRoutine());
        }

        private IEnumerator AutoCloseRoutine()
        {
            yield return new WaitForSeconds(closeDelay);
            isOpen = false;
        }
    }
}