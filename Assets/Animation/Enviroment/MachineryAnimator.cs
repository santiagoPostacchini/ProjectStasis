using UnityEngine;
using System.Collections;

public class MachineryAnimator : MonoBehaviour
{
    [Tooltip("Configuración de animación ScriptableObject")]
    public MachineryAnimationConfig config;

    [Tooltip("Si está activado, arranca en Start()")]
    public bool playOnStart = false;

    void Start()
    {
        if (playOnStart)
            PlayAssembly();
    }

    /// <summary>
    /// Llama a este método para iniciar el montaje
    /// (por ejemplo desde un trigger o evento de puzzle)
    /// </summary>
    public void PlayAssembly()
    {
        StartCoroutine(AnimateSequence());
    }

    private IEnumerator AnimateSequence()
    {
        foreach (var step in config.steps)
        {
            // Retraso antes de este paso
            if (step.delay > 0)
                yield return new WaitForSeconds(step.delay);

            // Valores de partida
            Vector3 startPos = step.fromPosition != Vector3.zero ? step.fromPosition : step.target.position;
            Vector3 startRot = step.fromRotation != Vector3.zero ? step.fromRotation : step.target.eulerAngles;

            float elapsed = 0f;
            while (elapsed < step.duration)
            {
                float t = elapsed / step.duration;
                float curved = step.curve.Evaluate(t);

                // Interpola posición y rotación
                step.target.position = Vector3.Lerp(startPos, step.toPosition, curved);
                step.target.rotation = Quaternion.Euler(Vector3.Lerp(startRot, step.toRotation, curved));

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Asegura valor final exacto
            step.target.position = step.toPosition;
            step.target.rotation = Quaternion.Euler(step.toRotation);
        }
    }
}
