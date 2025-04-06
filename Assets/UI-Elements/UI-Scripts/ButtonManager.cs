using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class ManagedButton
    {
        public string name;                        // Nombre identificador del botón
        public Button button;                      // Referencia al componente Button
        public TextMeshProUGUI label;              // Texto del botón (TMP)
        public Image background;                   // Fondo del botón
    }

    [Header("Lista de botones gestionados")]
    public List<ManagedButton> buttons = new List<ManagedButton>();

    // 🔹 Cambiar el texto del botón
    public void SetButtonText(string buttonName, string newText)
    {
        var btn = buttons.Find(b => b.name == buttonName);
        if (btn != null && btn.label != null)
        {
            btn.label.text = newText;
        }
        else
        {
            Debug.LogWarning($"No se encontró el botón o el texto de: {buttonName}");
        }
    }

    // 🔹 Activar o desactivar un botón
    public void SetButtonInteractable(string buttonName, bool isInteractable)
    {
        var btn = buttons.Find(b => b.name == buttonName);
        if (btn != null && btn.button != null)
        {
            btn.button.interactable = isInteractable;
        }
        else
        {
            Debug.LogWarning($"No se encontró el botón para: {buttonName}");
        }
    }

    // 🔹 Cambiar el color del texto
    public void SetTextColor(string buttonName, Color color)
    {
        var btn = buttons.Find(b => b.name == buttonName);
        if (btn != null && btn.label != null)
        {
            btn.label.color = color;
        }
        else
        {
            Debug.LogWarning($"No se encontró el texto de: {buttonName}");
        }
    }

    // 🔹 Cambiar el color del fondo del botón
    public void SetButtonBackgroundColor(string buttonName, Color color)
    {
        var btn = buttons.Find(b => b.name == buttonName);
        if (btn != null && btn.background != null)
        {
            btn.background.color = color;
        }
        else
        {
            Debug.LogWarning($"No se encontró el fondo del botón: {buttonName}");
        }
    }

    // 🔹 Desactivar todos los botones de la lista
    public void DisableAllButtons()
    {
        foreach (var btn in buttons)
        {
            if (btn.button != null)
            {
                btn.button.interactable = false;
            }
        }
    }
}
