//using UnityEngine;
//using System.Collections.Generic;

//public class CustomTag : MonoBehaviour
//{
//    public bool IsEnabled = true;
//    private void Start()
//    {

//    }

//    [SerializeField]
//    private List<string> tags = new List<string>();

//    public bool HasTag(string tag)
//    {
//        return tags.Contains(tag);
//    }

//    public IEnumerable<string> GetTags()
//    {
//        return tags;
//    }

//    public void Rename(int index, string tagName)
//    {
//        tags[index] = tagName;
//    }

//    public string GetAtIndex(int index)
//    {
//        return tags[index];
//    }

//    public int Count
//    {
//        get { return tags.Count; }
//    }
//}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente que marca a un GameObject con etiquetas personalizadas,
/// para que los detectores (ObstructionDetector) puedan filtrarlo.
/// </summary>
[DisallowMultipleComponent]
public class CustomTag : MonoBehaviour
{
    [Tooltip("Si está desactivado, ignora todas las etiquetas")]
    public bool IsEnabled = true;

    [Tooltip("Lista de etiquetas asociadas a este objeto")]
    [SerializeField] private List<string> tags = new List<string>();

    /// <summary>Lectura segura de las etiquetas.</summary>
    public IReadOnlyCollection<string> Tags => tags.AsReadOnly();

    /// <summary>¿Contiene esta etiqueta y está habilitado?</summary>
    public bool HasTag(string tag)
    {
        return IsEnabled && tags.Contains(tag);
    }

    /// <summary>Añade una etiqueta si no existe ya.</summary>
    public void AddTag(string tag)
    {
        if (!tags.Contains(tag))
            tags.Add(tag);
    }

    /// <summary>Elimina una etiqueta existente.</summary>
    public void RemoveTag(string tag)
    {
        tags.Remove(tag);
    }
}

