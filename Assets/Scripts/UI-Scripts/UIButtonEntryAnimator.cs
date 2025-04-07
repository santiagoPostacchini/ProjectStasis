using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIButtonEntryAnimator : MonoBehaviour
{
    public float startDelay = 0f;
    public float moveDistance = 100f;
    public float duration = 0.6f;
    public Vector2 direction = Vector2.left;

    private Vector3 initialPosition;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = transform.localPosition;
        canvasGroup.alpha = 0;
    }

    void Start()
    {
        StartCoroutine(AnimateEntry());
    }

    IEnumerator AnimateEntry()
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 startPos = initialPosition + (Vector3)(direction * moveDistance);
        Vector3 endPos = initialPosition;

        transform.localPosition = startPos;
        canvasGroup.alpha = 0;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0, 1, t / duration);
            transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            canvasGroup.alpha = progress;
            yield return null;
        }

        transform.localPosition = endPos;
        canvasGroup.alpha = 1;
    }
}
