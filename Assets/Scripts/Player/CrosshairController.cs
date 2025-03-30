using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public static CrosshairController Instance;
    [Header("Crosshair Settings")]
    public RectTransform crosshairRect;  // Assign the RectTransform of the crosshair
    public float enlargeSize = 150f;     // Size when interacting
    public float shrinkSize = 100f;      // Size when not interacting
    public float transitionSpeed = 0.1f; // Speed of size change

    private Vector2 targetSize;          // Target size of the crosshair
    private Coroutine scaleCoroutine;    // Reference to active coroutine
    private bool isEnlarged = false;     // Current state of the crosshair

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        targetSize = new Vector2(shrinkSize, shrinkSize);
        crosshairRect.sizeDelta = targetSize;
    }

    // Toggle between enlarge and shrink based on interactable state
    public void ToggleCrosshairSize(bool isInteractable)
    {
        if (isInteractable && !isEnlarged)
        {
            SetCrosshairSize(enlargeSize);
            isEnlarged = true;
        }
        else if (!isInteractable && isEnlarged)
        {
            SetCrosshairSize(shrinkSize);
            isEnlarged = false;
        }
    }

    // Set the target size and start the coroutine
    private void SetCrosshairSize(float size)
    {
        targetSize = new Vector2(size, size);

        // Stop any existing coroutine before starting a new one
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleCrosshair(targetSize));
    }

    // Coroutine to smoothly scale the crosshair
    private IEnumerator ScaleCrosshair(Vector2 targetSize)
    {
        Vector2 startSize = crosshairRect.sizeDelta;
        float elapsedTime = 0f;

        while (elapsedTime < transitionSpeed)
        {
            crosshairRect.sizeDelta = Vector2.Lerp(startSize, targetSize, elapsedTime / transitionSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final size to avoid small discrepancies
        crosshairRect.sizeDelta = targetSize;
    }
}
