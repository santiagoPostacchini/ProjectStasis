
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TechButtonMaterialInstance : MonoBehaviour
{
    [Header("M�scara personalizada para este bot�n")]
    public Texture2D customMask;

    void Awake()
    {
        Image img = GetComponent<Image>();

        if (img.material == null)
        {
            Debug.LogWarning("No se asign� material al Image.");
            return;
        }

        // Crear instancia �nica del material para este objeto
        img.material = Instantiate(img.material);

        // Asignar la nueva textura de m�scara
        if (customMask != null)
        {
            img.material.SetTexture("_MaskTex", customMask);
        }
    }
}
