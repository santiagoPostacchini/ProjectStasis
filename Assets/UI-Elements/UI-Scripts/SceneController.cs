using System.Collections.Generic;  // Para usar List<T>
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Tooltip("Lista de nombres de las escenas en el orden que prefieras.")]
    public List<string> sceneNames;

    /// <summary>
    /// Carga una escena espec�fica de la lista, dada su posici�n (�ndice).
    /// Por ejemplo, si sceneNames[0] = "MainMenu", sceneNames[1] = "Gameplay", etc.
    /// </summary>
    /// <param name="sceneIndex">�ndice dentro de la lista sceneNames</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneNames.Count)
        {
            SceneManager.LoadScene(sceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogError("�ndice de escena fuera de rango o lista vac�a.");
        }
    }

    /// <summary>
    /// Carga la siguiente escena en la lista, seg�n el �ndice de la escena actual.
    /// </summary>
    public void LoadNextScene()
    {
        // Consigue el nombre de la escena que se est� ejecutando en este momento
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Encuentra su posici�n en la lista
        int currentIndex = sceneNames.IndexOf(currentSceneName);

        if (currentIndex < 0)
        {
            Debug.LogError("La escena actual no se encuentra en la lista.");
            return;
        }

        // Suma 1 para ir a la siguiente, y si sobrepasa el �ltimo �ndice, vuelve a 0 (opcional)
        int nextIndex = (currentIndex + 1) % sceneNames.Count;

        SceneManager.LoadScene(sceneNames[nextIndex]);
    }
}
