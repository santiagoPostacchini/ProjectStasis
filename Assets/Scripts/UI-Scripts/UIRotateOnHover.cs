using UnityEngine;
using UnityEngine.EventSystems;

public class UIRotateOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotationSpeed = 120f;
    private bool rotating = false;

    public void OnPointerEnter(PointerEventData eventData) => rotating = true;
    public void OnPointerExit(PointerEventData eventData) => rotating = false;

    void Update()
    {
        if (rotating)
            transform.Rotate(0f, 0f, -rotationSpeed * Time.unscaledDeltaTime);
    }
}
