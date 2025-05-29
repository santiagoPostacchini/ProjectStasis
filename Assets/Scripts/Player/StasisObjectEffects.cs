using System.Collections;
using Events;
using NuevoInteractor;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class StasisObjectEffects : MonoBehaviour
    {
        [Header("Reticule UI")]
        [Tooltip("Lista de imágenes de la mira; todas se animan")]
        public Image[] reticleImages;

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

        void Start()
        {
            foreach (var img in reticleImages)
            {
                if (!img) continue;
                img.rectTransform.localScale     = Vector3.one * normalScale;
                img.color                        = normalColor;
                img.rectTransform.localEulerAngles = Vector3.zero;
            }
        }

        public void HandleVisualStasisFeedback(IStasis lookedStasisObject, bool isGrabbing)
        {
            bool hitStasis = (lookedStasisObject != null);
            var lookedPhysicsObject = lookedStasisObject as NewPhysicsBox;
            var lastPhysicsObject   = _lastLookedStasisObject as NewPhysicsBox;

            // 1) Arranca o detiene la animación de la mira
            if (hitStasis != _isAiming)
            {
                _isAiming = hitStasis;
                if (_animCoroutine != null) StopCoroutine(_animCoroutine);
                _animCoroutine = StartCoroutine(AnimateReticle(_isAiming));
            }

            // 2) Cambio de objeto apuntado
            if (lookedStasisObject != _lastLookedStasisObject)
            {
                // 2a) Si dejamos de mirar un staseable, restauro y paro el tick
                if (_lastLookedStasisObject != null && lastPhysicsObject)
                {
                    if (!lastPhysicsObject.isFreezed)
                        lastPhysicsObject.SetOutlineThickness(0f);

                    EventManager.TriggerEvent(tickingStopEvent, gameObject);
                }

                // 2b) Si miramos uno nuevo y no está congelado ni agarrando, highlight + start tick + sonido select
                if (lookedPhysicsObject && !lookedPhysicsObject.isFreezed && !isGrabbing)
                {
                    lookedPhysicsObject.SetOutlineThickness(1.01f);
                    EventManager.TriggerEvent(tickingStartEvent, gameObject);
                    EventManager.TriggerEvent(selectEvent,      gameObject);
                }
            }

            // 3) Si empiezo a agarrar, detengo el tick inmediatamente
            if (isGrabbing)
            {
                EventManager.TriggerEvent(tickingStopEvent, gameObject);
            }

            // 4) Actualizo última referencia
            _lastLookedStasisObject = lookedStasisObject;
        }

        private IEnumerator AnimateReticle(bool highlight)
        {
            float start    = highlight ? 0f : 1f;
            float end      = highlight ? 1f : 0f;
            float progress = start;
            float duration = 1f / Mathf.Max(0.01f, animSpeed);

            while (Mathf.Abs(progress - end) > 0.01f)
            {
                progress += (highlight ? 1 : -1) * (Time.deltaTime / duration);
                float t = animCurve.Evaluate(Mathf.Clamp01(progress));

                foreach (var img in reticleImages)
                {
                    if (!img) continue;
                    // escala
                    float s = Mathf.Lerp(normalScale, highlightScale, t);
                    img.rectTransform.localScale = Vector3.one * s;
                    // color
                    img.color = Color.Lerp(normalColor, highlightColor, t);
                    // rotación
                    float angle = img.rectTransform.localEulerAngles.z + rotationSpeed * t * Time.deltaTime;
                    img.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
                }
                yield return null;
            }

            // asegurar valores finales
            foreach (var img in reticleImages)
            {
                if (!img) continue;
                float s   = Mathf.Lerp(normalScale, highlightScale, end);
                img.rectTransform.localScale = Vector3.one * s;
                img.color                    = Color.Lerp(normalColor, highlightColor, end);
            }
        }
    }
}