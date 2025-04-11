using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Cambia a una escena por nombre
    public void ChangeSceneByString(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    // Cambia a una escena por índice en el Build Settings
    public void ChangeSceneByIndex(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }

    // Reinicia la escena actual
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}