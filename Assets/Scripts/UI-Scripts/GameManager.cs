using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneName = "SceneToLoad";
    [SerializeField] private float delayBeforeSceneLoad = 2f;

    [Header("Video Components")]
    [SerializeField] private VideoPlayer videoPlayer;         // Asigna aquí tu VideoPlayer
    [SerializeField] private RawImage videoRawImage;          // Asigna aquí el RawImage (que muestra el video)
    [SerializeField] private RenderTexture videoRenderTexture;// Asigna la RenderTexture del video

    // Método que llamará el botón OnClick
    public void OnButtonClick()
    {
        StartCoroutine(ShowVideoAndLoadScene());
    }

    private IEnumerator ShowVideoAndLoadScene()
    {
        if (videoPlayer != null)
        {
            // Aseguramos que el VideoPlayer use la RenderTexture (por si no lo configuraste en el Inspector)
            videoPlayer.targetTexture = videoRenderTexture;

            // Activamos el objeto que contiene la Raw Image (para que se vea el video)
            if (videoRawImage != null)
            {
                videoRawImage.gameObject.SetActive(true);
                videoRawImage.texture = videoRenderTexture;
            }

            // Preparamos el video antes de reproducir
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);

            // Reproducimos el video
            videoPlayer.Play();

            // Esperamos a que el video termine de reproducirse
            // (Si el clip está configurado en Loop, esta condición nunca se cumplirá)
            yield return new WaitUntil(() => !videoPlayer.isPlaying);

            // Opcional: desactivamos el RawImage otra vez, si no quieres que quede en pantalla
            if (videoRawImage != null)
            {
                videoRawImage.gameObject.SetActive(false);
            }
        }
        else
        {
            // Si por algún motivo no hay VideoPlayer, hacemos un delay "de cortesía"
            yield return new WaitForSeconds(delayBeforeSceneLoad);
        }

        // Finalmente, cargamos la siguiente escena
        SceneManager.LoadScene(sceneName);
    }
}
