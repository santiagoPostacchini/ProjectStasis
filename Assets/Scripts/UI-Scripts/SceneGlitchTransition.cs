using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGlitchTransition : MonoBehaviour
{
    public Material glitchMaterial; // material con shader de glitch
    public float transitionDuration = 1.5f;
    public string targetScene;

    private bool transitioning = false;
    private float time = 0f;

    void Start()
    {
        if (glitchMaterial != null)
            glitchMaterial.SetFloat("_EffectActive", 0); // asegurate de iniciar apagado
    }

    public void TriggerTransition()
    {
        if (!transitioning)
        {
            transitioning = true;
            time = 0f;
            StartCoroutine(GlitchAndLoad());
        }
    }

    private System.Collections.IEnumerator GlitchAndLoad()
    {
        // Activar efecto glitch
        if (glitchMaterial != null)
            glitchMaterial.SetFloat("_EffectActive", 1);

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(targetScene);
    }
}
