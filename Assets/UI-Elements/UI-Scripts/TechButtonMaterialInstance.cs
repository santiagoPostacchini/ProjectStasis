
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TechButtonMaterialInstance : MonoBehaviour
{
    [Header("Máscara personalizada para este botón")]
    public Texture2D customMask;

    void Awake()
    {
        Image img = GetComponent<Image>();

        if (img.material == null)
        {
            Debug.LogWarning("No se asignó material al Image.");
            return;
        }

        // Crear instancia única del material para este objeto
        img.material = Instantiate(img.material);

        // Asignar la nueva textura de máscara
        if (customMask != null)
        {
            img.material.SetTexture("_MaskTex", customMask);
        }
    }
}
