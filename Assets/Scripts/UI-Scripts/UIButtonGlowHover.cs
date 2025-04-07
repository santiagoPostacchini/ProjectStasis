using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonGlowHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image glow;
    public float fadeDuration = 0.3f;

    private Coroutine current;

    void Awake()
    {
        if (glow != null)
            glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (glow != null)
        {
            if (current != null) StopCoroutine(current);
            current = StartCoroutine(FadeGlow(1f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (glow != null)
        {
            if (current != null) StopCoroutine(current);
            current = StartCoroutine(FadeGlow(0f));
        }
    }

    System.Collections.IEnumerator FadeGlow(float targetAlpha)
    {
        Color startColor = glow.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startColor.a, targetAlpha, t / fadeDuration);
            glow.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        glow.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
