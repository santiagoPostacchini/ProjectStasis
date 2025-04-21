/*─────────────────────────────────────────────────────────────────────────────
 * TimeTravelPostFXManager.cs
 * ---------------------------------------------------------------------------
 * Controla la “explosión” de post‑proceso, sonido, snapshots, FX extras y shake
 * en tu salto temporal:
 *   • Sube/baja el Weight del Volume global.
 *   • Cierra el Depth‑of‑Field (Gaussian) para un blur extremo.
 *   • Aumenta la Chromatic Aberration.
 *   • Aplica Lens Distortion tipo “túnel”.
 *   • Añade Vignette, Bloom flash y Film Grain.
 *   • Reproduce un SFX de whoosh y mezcla snapshots de audio.
 *   • Dispara un CameraShake simple antes del fade.
 *
 * Diseñado para URP (2021.2+ / 2022 LTS).
 * ¡Añade este script a un GameObject persistente (ej. Main Camera)!"
 *────────────────────────────────────────────────────────────────────────────*/

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class TimeTravelPostFXManager : MonoBehaviour
{
    /* ══════════════  CONFIGURACIÓN GENERAL  ══════════════ */
    [Header("Volume Global (URP)")]
    [Tooltip("Arrastra aquí tu Volume Global. Si lo dejas vacío, el script intentará encontrar uno en la escena.")]
    [SerializeField] private Volume volume;

    [Header("Audio (SFX)")]
    [Tooltip("AudioSource con el clip de whoosh. Se reproducirá al iniciar el blur.")]
    [SerializeField] private AudioSource sfxSource;

    [Header("AudioMixer y Snapshots")]
    [Tooltip("AudioMixer que contiene los snapshots de Normal y TimeTravel.")]
    [SerializeField] private AudioMixer mixer;
    [Tooltip("Snapshot de audio en estado normal.")]
    [SerializeField] private AudioMixerSnapshot snapNormal;
    [Tooltip("Snapshot de audio durante el viaje temporal.")]
    [SerializeField] private AudioMixerSnapshot snapTimeTravel;

    [Header("Camera Shake")]
    [Tooltip("Referencia al script CameraShake para temblor de cámara.")]
    [SerializeField] private CameraShake cameraShake;
    [Tooltip("Duración del shake en segundos.")]
    [SerializeField] private float shakeDuration = 0.2f;
    [Tooltip("Magnitud máxima del shake.")]
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Peso máximo del Volume")]
    [Range(0f, 1f)] public float maxWeight = 1f;

    /* ══════════════  DEPTH OF FIELD  ══════════════ */
    [Header("Depth‑of‑Field (Gaussian)")]
    [Tooltip("Posición de inicio del blur intenso (m).")]
    public float closeStart = 0.02f;
    [Tooltip("Posición de fin del blur intenso (m).")]
    public float closeEnd = 0.05f;

    /* ══════════════  CHROMATIC ABERRATION  ══════════════ */
    [Header("Chromatic Aberration")]
    [Tooltip("Intensidad máxima para aberración cromática.")]
    [Range(0f, 1f)] public float maxChromAberration = 0.9f;

    /* ══════════════  LENS DISTORTION  ══════════════ */
    [Header("Lens Distortion")]
    [Tooltip("Intensidad negativa = efecto 'túnel'.")]
    [Range(-1f, 1f)] public float maxLensDistort = -0.6f;
    [Tooltip("Escala cuando el efecto está al máximo (para evitar bordes negros).")]
    [Range(0.7f, 1f)] public float closeScale = 0.9f;

    /* ══════════════  VIGNETTE  ══════════════ */
    [Header("Vignette")]
    [Tooltip("Intensidad máxima de viñeta.")]
    [Range(0f, 1f)] public float maxVignetteIntensity = 0.6f;

    /* ══════════════  BLOOM  ══════════════ */
    [Header("Bloom Flash")]
    [Tooltip("Intensidad máxima de bloom durante el flash.")]
    public float maxBloomIntensity = 3f;

    /* ══════════════  FILM GRAIN  ══════════════ */
    [Header("Film Grain")]
    [Tooltip("Intensidad máxima de grano de película.")]
    [Range(0f, 1f)] public float maxGrainIntensity = 0.3f;

    /* ══════════════  REFERENCIAS INTERNAS  ══════════════ */
    private DepthOfField dof;
    private ChromaticAberration ca;
    private LensDistortion ld;
    private Vignette vgn;
    private Bloom bloom;
    private FilmGrain grain;

    private Coroutine fadeRoutine;

    #region Inicialización
    private void Awake()
    {
        // Volumen
        if (volume == null)
            volume = GetComponent<Volume>() ?? FindObjectOfType<Volume>();
        if (volume == null)
        {
            Debug.LogError("TimeTravelPostFXManager: No Volume found.");
            enabled = false; return;
        }
        // Overrides POST FX
        volume.profile.TryGet(out dof);
        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out ld);
        volume.profile.TryGet(out vgn);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out grain);
        if (dof == null || ca == null || ld == null || vgn == null || bloom == null || grain == null)
        {
            Debug.LogError("TimeTravelPostFXManager: Missing one or more postfx overrides.");
            enabled = false;
        }
    }
    #endregion

    #region API pública
    /// <summary>Inicia blur, sonido, shake y transición de snapshots.</summary>
    public void BeginBlur(float fadeIn = 0.25f)
    {
        if (!enabled) return;
        // Camera Shake
        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeMagnitude);
        // SFX
        if (sfxSource != null && !sfxSource.isPlaying)
            sfxSource.Play();
        // Start Fade
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(0f, 1f, fadeIn));
    }

    /// <summary>Finaliza blur y restaura valores normales.</summary>
    public void EndBlur(float fadeOut = 0.4f)
    {
        if (!enabled) return;
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(1f, 0f, fadeOut));
    }
    #endregion

    #region Corutina de interpolación
    private IEnumerator Fade(float from, float to, float seconds)
    {
        // Guardar valores originales
        float origStart = dof.gaussianStart.value;
        float origEnd = dof.gaussianEnd.value;
        float origCA = ca.intensity.value;
        float origLD = ld.intensity.value;
        float origScale = ld.scale.value;
        float origVgn = vgn.intensity.value;
        float origBloom = bloom.intensity.value;
        float origGrain = grain.intensity.value;

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(from, to, elapsed / seconds);

            // Visual FX
            volume.weight = Mathf.Lerp(0f, maxWeight, k);
            dof.gaussianStart.value = Mathf.Lerp(origStart, closeStart, k);
            dof.gaussianEnd.value = Mathf.Lerp(origEnd, closeEnd, k);
            ca.intensity.value = Mathf.Lerp(origCA, maxChromAberration, k);
            ld.intensity.value = Mathf.Lerp(origLD, maxLensDistort, k);
            ld.scale.value = Mathf.Lerp(origScale, closeScale, k);
            vgn.intensity.value = Mathf.Lerp(origVgn, maxVignetteIntensity, k);
            bloom.intensity.value = Mathf.Lerp(origBloom, maxBloomIntensity, k);
            grain.intensity.value = Mathf.Lerp(origGrain, maxGrainIntensity, k);

            // Audio Snapshots
            if (mixer != null && snapNormal != null && snapTimeTravel != null)
                mixer.TransitionToSnapshots(
                    new[] { snapNormal, snapTimeTravel },
                    new[] { 1f - k, k },
                    0f);

            yield return null;
        }

        // Ajuste final
        volume.weight = Mathf.Lerp(0f, maxWeight, to);
        dof.gaussianStart.value = (to > 0f) ? closeStart : origStart;
        dof.gaussianEnd.value = (to > 0f) ? closeEnd : origEnd;
        ca.intensity.value = (to > 0f) ? maxChromAberration : origCA;
        ld.intensity.value = (to > 0f) ? maxLensDistort : origLD;
        ld.scale.value = (to > 0f) ? closeScale : origScale;
        vgn.intensity.value = (to > 0f) ? maxVignetteIntensity : origVgn;
        bloom.intensity.value = (to > 0f) ? maxBloomIntensity : origBloom;
        grain.intensity.value = (to > 0f) ? maxGrainIntensity : origGrain;

        // Restaurar audio a Normal
        if (to == 0f && mixer != null && snapNormal != null)
            mixer.TransitionToSnapshots(new[] { snapNormal }, new[] { 1f }, 0f);
    }
    #endregion
}
