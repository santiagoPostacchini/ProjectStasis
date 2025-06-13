using System.Collections;
using UnityEngine;

/// <summary>
/// Cámara shake básico sin Cinemachine.
/// Añade este componente a tu cámara principal y llama a Shake().
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeRoutine;


    private void Awake()
    {
        originalPos = transform.localPosition;
    }
   
    /// <summary>
    /// Inicia el efecto de sacudida de cámara.
    /// </summary>
    /// <param name="duration">Duración total de la sacudida en segundos.</param>
    /// <param name="magnitude">Magnitud máxima del movimiento.</param>
    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void Shake10Seconds()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeCoroutine(16, 1));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // porcentaje completado (0→1)
            float progress = elapsed / duration;
            // atenuación al final (puedes usar Mathf.SmoothStep)
            float damper = 1f - Mathf.Clamp01(progress);

            // genera un offset aleatorio
            float offsetX = (Random.value * 2f - 1f) * magnitude * damper;
            float offsetY = (Random.value * 2f - 1f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // restaurar posición original
        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
