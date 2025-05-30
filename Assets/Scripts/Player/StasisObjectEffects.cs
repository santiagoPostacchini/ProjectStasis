using Events;
using NuevoInteractor;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class StasisObjectEffects : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Curva de easing para la animación (entrada/salida)")]
        public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Tooltip("Velocidad de la progresión (1 = 1 segundo para ir de 0→1)")]
        public float animSpeed = 2f;

        [Header("Scale Settings")]
        [Tooltip("Escala en reposo")]
        public float normalScale = 1f;
        [Tooltip("Escala al highlight")]
        public float highlightScale = 1.2f;

        [Header("Color Settings")]
        [Tooltip("Color en reposo")]
        public Color normalColor = Color.white;
        [Tooltip("Color al highlight")]
        public Color highlightColor = Color.cyan;

        [Header("Rotation Settings")]
        [Tooltip("Velocidad de giro máxima (grados/segundo)")]
        public float rotationSpeed = 90f;

        [Header("Eventos de Sonido")]
        [Tooltip("Evento que dispara el tick al apuntar")]
        public string tickingStartEvent = "Ticking.PulseStart";
        [Tooltip("Evento que detiene el tick")]
        public string tickingStopEvent  = "Ticking.PulseStop";
        [Tooltip("Evento de selección de objeto staseable")]
        public string selectEvent       = "SelectStasiable";

        private Coroutine _animCoroutine;
        private bool      _isAiming;
        private IStasis  _lastLookedStasisObject;

        public void HandleVisualStasisFeedback(IStasis lookedStasisObject, bool isGrabbing)
        {
            var lookedPhysicsObject = lookedStasisObject as NewPhysicsBox;
            var lastPhysicsObject   = _lastLookedStasisObject as NewPhysicsBox;
            
            if (lookedStasisObject != _lastLookedStasisObject)
            {
                if (_lastLookedStasisObject != null && lastPhysicsObject)
                {
                    if (!lastPhysicsObject.isFreezed)
                        lastPhysicsObject.SetOutlineThickness(0f);

                    EventManager.TriggerEvent(tickingStopEvent, gameObject);
                }
                
                if (lookedPhysicsObject && !lookedPhysicsObject.isFreezed && !isGrabbing)
                {
                    lookedPhysicsObject.SetOutlineThickness(1.01f);
                    EventManager.TriggerEvent(tickingStartEvent, gameObject);
                    EventManager.TriggerEvent(selectEvent,      gameObject);
                }
            }
            
            if (isGrabbing)
            {
                EventManager.TriggerEvent(tickingStopEvent, gameObject);
            }
            
            _lastLookedStasisObject = lookedStasisObject;
        }
    }
}