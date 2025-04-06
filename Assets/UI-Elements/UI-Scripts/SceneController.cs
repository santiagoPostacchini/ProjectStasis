using System.Collections.Generic;  // Para usar List<T>
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Tooltip("Lista de nombres de las escenas en el orden que prefieras.")]
    public List<string> sceneNames;

    /// <summary>
    /// Carga una escena específica de la lista, dada su posición (índice).
    /// Por ejemplo, si sceneNames[0] = "MainMenu", sceneNames[1] = "Gameplay", etc.
    /// </summary>
    /// <param name="sceneIndex">Índice dentro de la lista sceneNames</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            SceneManager.LoadScene(sceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogError("Índice de escena fuera de rango o lista vacía.");
        }
    }

    /// <summary>
    /// Carga la siguiente escena en la lista, según el índice de la escena actual.
    /// </summary>
    public void LoadNextScene()
    {
        // Consigue el nombre de la escena que se esté ejecutando en este momento
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Encuentra su posición en la lista
        int currentIndex = sceneNames.IndexOf(currentSceneName);

        if (currentIndex < 0)
        {
            Debug.LogError("La escena actual no se encuentra en la lista.");
            return;
        }

        // Suma 1 para ir a la siguiente, y si sobrepasa el último índice, vuelve a 0 (opcional)
        int nextIndex = (currentIndex + 1) % sceneNames.Count;

        SceneManager.LoadScene(sceneNames[nextIndex]);
    }
}
