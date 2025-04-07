using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneGlitchTransition : MonoBehaviour
{
    [Header("UI y Material")]
    public GameObject glitchOverlay;
    public Material glitchMaterialOriginal;

    [Header("Parámetros")]
    public float fadeDuration = 0.5f;
    public float totalDuration = 2f;
    public string targetScene;

    private Material glitchMaterialInstance;
    private CanvasGroup canvasGroup;
    private bool transitioning = false;

    void Start()
    {
        if (glitchOverlay != null)
        {
            canvasGroup = glitchOverlay.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            glitchOverlay.SetActive(false);
        }

        if (glitchMaterialOriginal != null)
        {
            glitchMaterialInstance = Instantiate(glitchMaterialOriginal); //  instancia única
            glitchMaterialInstance.SetFloat("_EffectActive", 0);
            glitchMaterialInstance.SetFloat("_GlitchStrength", 0);

            // Asignar el nuevo material instanciado al RawImage
            var rawImage = glitchOverlay.GetComponent<RawImage>();
            if (rawImage != null)
                rawImage.material = glitchMaterialInstance;
        }
    }

    public void TriggerTransition()
    {
        if (!transitioning)
        {
            transitioning = true;
            StartCoroutine(AnimateGlitchAndLoad());
        }
    }

    private IEnumerator AnimateGlitchAndLoad()
    {
        glitchOverlay.SetActive(true);
        Debug.Log("TRANSICIÓN INICIADA");

        glitchMaterialInstance.SetFloat("_EffectActive", 1);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = normalized;
                Debug.Log("FADE: " + normalized);
            }

            if (glitchMaterialInstance != null)
                glitchMaterialInstance.SetFloat("_GlitchStrength", normalized);

            yield return null;
        }

        Debug.Log("FADE COMPLETO — esperando " + (totalDuration - fadeDuration) + " segundos");

        yield return new WaitForSecondsRealtime(totalDuration - fadeDuration);

        Debug.Log("Cargando escena: " + targetScene);
        ResetGlitch();

        SceneManager.LoadScene(targetScene);
    }


    private void ResetGlitch()
    {
        if (glitchMaterialInstance != null)
        {
            glitchMaterialInstance.SetFloat("_EffectActive", 0);
            glitchMaterialInstance.SetFloat("_GlitchStrength", 0);
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0;

        if (glitchOverlay != null)
            glitchOverlay.SetActive(false);

        transitioning = false;
    }
}
