using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class UIButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI label;
    public Image background;
    public Material targetMaterial;
    public string shaderParamName = "_EffectActive";

    [Header("Sprites")]
    public Sprite backgroundOnHover;
    public Sprite backgroundDefault;

    [Header("Color Settings")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.yellow;

    [Header("Animation Settings")]
    public float hoverScale = 1.05f;
    public float pressedScale = 0.9f;
    public float animationSpeed = 10f;
    public bool animate = true;

    [Header("Audio")]
    public AudioClip hoverSound;
    private AudioSource audioSource;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    private bool isPressed = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        audioSource = GetComponent<AudioSource>();
        if (hoverSound != null)
        {
            audioSource.playOnAwake = false;
            audioSource.clip = hoverSound;
        }

        if (targetMaterial != null)
        {
            targetMaterial = Instantiate(targetMaterial);
            background.material = targetMaterial;
            targetMaterial.SetFloat(shaderParamName, 0);
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (animate && !isPressed)
            targetScale = originalScale * hoverScale;

        if (label != null)
            label.color = hoverTextColor;
        if (background != null && backgroundOnHover != null)
            background.sprite = backgroundOnHover;

        if (targetMaterial != null)
            targetMaterial.SetFloat(shaderParamName, 1);

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        targetScale = originalScale;

        if (label != null)
            label.color = normalTextColor;
        if (background != null && backgroundDefault != null)
            background.sprite = backgroundDefault;

        if (targetMaterial != null)
            targetMaterial.SetFloat(shaderParamName, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        if (animate)
            targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (animate)
            targetScale = isHovered ? originalScale * hoverScale : originalScale;
    }
}
