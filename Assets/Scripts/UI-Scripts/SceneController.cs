using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Optional Glitch Transition")]
    public SceneGlitchTransition glitchTransition; // Asignar desde el Inspector

    private string lastScene;

    void Awake()
    {
        // Singleton básico
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneDelayed(sceneName, delay));
    }

    private IEnumerator LoadSceneDelayed(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void LoadLastScene()
    {
        if (!string.IsNullOrEmpty(lastScene))
            SceneManager.LoadScene(lastScene);
    }

    public void LoadMenuScene()
    {
        LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    // 🔥 NUEVO: Transición con glitch sin usar GameObject.Find
    public void LoadSceneWithGlitch(string sceneName)
    {
        if (glitchTransition != null)
        {
            glitchTransition.targetScene = sceneName;
            glitchTransition.TriggerTransition();
        }
        else
        {
            Debug.LogWarning("No glitch transition assigned. Loading scene directly.");
            LoadScene(sceneName);
        }
    }
}
