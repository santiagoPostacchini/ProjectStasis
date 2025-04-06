using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI label;
    public Image background;

    [Header("Color Settings")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.yellow;
    public Color normalBackgroundColor = Color.white;
    public Color hoverBackgroundColor = Color.gray;

    [Header("Animation Settings")]
    public float pressedScale = 0.9f;
    public float animationSpeed = 10f; // velocidad de interpolación

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Interpolación suave tipo UI técnica
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (label != null)
            label.color = hoverTextColor;
        if (background != null)
            background.color = hoverBackgroundColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (label != null)
            label.color = normalTextColor;
        if (background != null)
            background.color = normalBackgroundColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetScale = originalScale;
    }
}
