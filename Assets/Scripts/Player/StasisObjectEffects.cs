using System.Collections;
using NuevoInteractor;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class StasisObjectEffects : MonoBehaviour
    {
        [Header("Reticule UI")] [Tooltip("Lista de imágenes de la mira; todas se animan")]
        public Image[] reticleImages;

        [Header("Animation Settings")] [Tooltip("Curva de easing para la animación (entrada/salida)")]
        public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("Velocidad de la progresión (1 = 1 segundo para ir de 0→1)")]
        public float animSpeed = 2f;

        [Header("Scale Settings")] [Tooltip("Escala en reposo")]
        public float normalScale = 1f;

        [Tooltip("Escala al highlight")] public float highlightScale = 1.2f;

        [Header("Color Settings")] [Tooltip("Color en reposo")]
        public Color normalColor = Color.white;

        [Tooltip("Color al highlight")] public Color highlightColor = Color.cyan;

        [Header("Rotation Settings")] [Tooltip("Velocidad de giro máxima (grados/segundo)")]
        public float rotationSpeed = 90f;

        [Header("Tick Sound")] [Tooltip("Nombre exacto del sonido en AudioManager (ambientSounds)")]
        public string tickingSoundName = "Ticking.Pulse";

        private Coroutine _animCoroutine;
        private bool _isAiming;

        void Start()
        {
            foreach (var img in reticleImages)
            {
                if (!img) continue;
                img.rectTransform.localScale = Vector3.one * normalScale;
                img.color = normalColor;
                img.rectTransform.localEulerAngles = Vector3.zero;
            }
        }
        
        private IStasis _lastLookedStasisObject;

        public void HandleVisualStasisFeedback(IStasis lookedStasisObject, bool isGrabbing)
        {
            bool hitStasis = (lookedStasisObject != null);
            var lookedPhysicsObject = lookedStasisObject as NewPhysicsBox;
            var lastPhysicsObject = _lastLookedStasisObject as NewPhysicsBox;

            // Cambio en el highlight de la mira
            if (hitStasis != _isAiming)
            {
                _isAiming = hitStasis;
                if (_animCoroutine != null) StopCoroutine(_animCoroutine);
                _animCoroutine = StartCoroutine(AnimateReticle(_isAiming));
            }

            // Solo ejecuta al apuntar a un nuevo objeto
            if (lookedStasisObject != _lastLookedStasisObject)
            {
                // Si dejé de mirar un objeto staseable, restablezco su estado anterior
                if (_lastLookedStasisObject != null && lastPhysicsObject)
                {
                    if (!lastPhysicsObject.isFreezed)
                    {
                        lastPhysicsObject.SetOutlineThickness(0f);
                    }
                    AudioManager.Instance?.StopAmbient(tickingSoundName);
                }

                // Ahora, si miro un nuevo objeto válido, aplico highlight y sonido
                if (lookedPhysicsObject && !lookedPhysicsObject.isFreezed && !isGrabbing)
                {
                    lookedPhysicsObject.SetOutlineThickness(1.01f);
                    AudioManager.Instance?.PlayAmbient(tickingSoundName);
                    AudioManager.Instance?.PlaySfx("SelectStasiable");
                }
            }

            // Si estoy agarrando, detengo el sonido inmediatamente
            if (isGrabbing)
            {
                AudioManager.Instance?.StopAmbient(tickingSoundName);
            }

            // Actualiza la referencia del último objeto mirado
            _lastLookedStasisObject = lookedStasisObject;
        }

        private IEnumerator AnimateReticle(bool highlight)
        {
            float start = highlight ? 0f : 1f;
            float end = highlight ? 1f : 0f;
            float animProgress = start;
            float duration = 1f / Mathf.Max(0.01f, animSpeed);

            while (Mathf.Abs(animProgress - end) > 0.01f)
            {
                animProgress = Mathf.MoveTowards(animProgress, end, Time.deltaTime / duration);
                float t = animCurve.Evaluate(animProgress);

                foreach (var img in reticleImages)
                {
                    if (!img) continue;
                    // Escala
                    float s = Mathf.Lerp(normalScale, highlightScale, t);
                    img.rectTransform.localScale = Vector3.one * s;
                    // Color
                    img.color = Color.Lerp(normalColor, highlightColor, t);
                    // Rotación
                    float angle = img.rectTransform.localEulerAngles.z + rotationSpeed * t * Time.deltaTime;
                    img.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
                }

                yield return null;
            }

            // Asegura valores finales exactos
            foreach (var img in reticleImages)
            {
                if (!img) continue;
                float s = Mathf.Lerp(normalScale, highlightScale, end);
                img.rectTransform.localScale = Vector3.one * s;
                img.color = Color.Lerp(normalColor, highlightColor, end);
            }
        }
    }
}