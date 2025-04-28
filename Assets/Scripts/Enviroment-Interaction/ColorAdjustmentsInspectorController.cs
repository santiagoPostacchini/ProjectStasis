using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class FullGradingController : MonoBehaviour
{
    [Header("Asigna tu Volume (debe tener overrides de Color Adjustments, Lift Gamma Gain y Shadows Midtones Highlights)")]
    public Volume volume;

    // Componentes de post-procesado
    ColorAdjustments ca;
    LiftGammaGain lgg;                       // Lift/Gamma/Gain :contentReference[oaicite:0]{index=0}
    ShadowsMidtonesHighlights smh;           // Shadows/Midtones/Highlights :contentReference[oaicite:1]{index=1}

    // — Basic Color Adjustments —
    [Space, Header("— Basic Color Adjustments —")]
    [Tooltip("Exposición (-2 … +2)")]
    [Range(-2f, 2f)] public float postExposure = 0f;
    [Tooltip("Contraste (-100 … +100)")]
    [Range(-100f, 100f)] public float contrast = 0f;
    [Tooltip("Hue Shift (-180 … +180)")]
    [Range(-180f, 180f)] public float hueShift = 0f;
    [Tooltip("Saturación (-100 … +100)")]
    [Range(-100f, 100f)] public float saturation = 0f;
    [Tooltip("Color Filter")]
    public Color colorFilter = Color.white;

    // — Lift / Gamma / Gain —
    [Space, Header("— Lift / Gamma / Gain —")]
    [ColorUsage(true, true)]
    public Color lift = new Color(0.6f, 0.75f, 1.1f, 0f);
    [ColorUsage(true, true)]
    public Color gamma = new Color(0.95f, 1f, 1f, 0f);
    [ColorUsage(true, true)]
    public Color gain = new Color(1.25f, 1f, 0.9f, 0f);

    // — Shadows / Midtones / Highlights —
    [Space, Header("— Shadows / Midtones / Highlights —")]
    [ColorUsage(true, true)]
    public Color shadows = new Color(0.2f, 0.6f, 0.75f, 0f);
    [Range(0f, 1f)] public float shadowsStart = 0f;
    [Range(0f, 1f)] public float shadowsEnd = 0.35f;

    [ColorUsage(true, true)]
    public Color midtones = new Color(0.95f, 0.9f, 0.85f, 0f);

    [ColorUsage(true, true)]
    public Color highlights = new Color(1f, 0.3f, 0.3f, 0f);
    [Range(0f, 1f)] public float highlightsStart = 0.7f;
    [Range(0f, 1f)] public float highlightsEnd = 1f;

    void OnEnable() => ApplyAll();
    void OnValidate() => ApplyAll();

    void ApplyAll()
    {
        if (volume == null || volume.profile == null) return;

        // — Color Adjustments —
        if (volume.profile.TryGet<ColorAdjustments>(out ca))
        {
            ca.active = true;
            ca.postExposure.overrideState = true;
            ca.contrast.overrideState = true;
            ca.hueShift.overrideState = true;
            ca.saturation.overrideState = true;
            ca.colorFilter.overrideState = true;

            ca.postExposure.value = postExposure;
            ca.contrast.value = contrast;
            ca.hueShift.value = hueShift;
            ca.saturation.value = saturation;
            ca.colorFilter.value = colorFilter;
        }

        // — Lift / Gamma / Gain —
        if (volume.profile.TryGet<LiftGammaGain>(out lgg))
        {
            lgg.active = true;
            lgg.lift.overrideState = true;
            lgg.gamma.overrideState = true;
            lgg.gain.overrideState = true;

            lgg.lift.value = lift;
            lgg.gamma.value = gamma;
            lgg.gain.value = gain;
        }

        // — Shadows / Midtones / Highlights —
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out smh))
        {
            smh.active = true;
            smh.shadows.overrideState = true;
            smh.shadowsStart.overrideState = true;
            smh.shadowsEnd.overrideState = true;
            smh.midtones.overrideState = true;
            smh.highlights.overrideState = true;
            smh.highlightsStart.overrideState = true;
            smh.highlightsEnd.overrideState = true;

            smh.shadows.value = shadows;
            smh.shadowsStart.value = shadowsStart;
            smh.shadowsEnd.value = shadowsEnd;
            smh.midtones.value = midtones;
            smh.highlights.value = highlights;
            smh.highlightsStart.value = highlightsStart;
            smh.highlightsEnd.value = highlightsEnd;
        }
    }
}
