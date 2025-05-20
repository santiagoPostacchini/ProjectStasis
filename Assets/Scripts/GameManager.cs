using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneName = "SceneToLoad";
    [SerializeField] private int index;
    [SerializeField] private float delayBeforeSceneLoad = 2f;

    [Header("Video Components")]
    [SerializeField] private VideoPlayer videoPlayer;         // Asigna aqu� tu VideoPlayer
    [SerializeField] private RawImage videoRawImage;          // Asigna aqu� el RawImage (que muestra el video)
    [SerializeField] private RenderTexture videoRenderTexture;// Asigna la RenderTexture del video

    // M�todo que llamar� el bot�n OnClick
    public void OnButtonClick()
    {
        StartCoroutine(ShowVideoAndLoadScene());
    }

    private IEnumerator ShowVideoAndLoadScene()
    {
        if (videoPlayer)
        {
            // Aseguramos que el VideoPlayer use la RenderTexture (por si no lo configuraste en el Inspector)
            videoPlayer.targetTexture = videoRenderTexture;

            // Activamos el objeto que contiene la Raw Image (para que se vea el video)
            if (videoRawImage)
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
            // (Si el clip est� configurado en Loop, esta condici�n nunca se cumplir�)
            yield return new WaitUntil(() => !videoPlayer.isPlaying);

            // Opcional: desactivamos el RawImage otra vez, si no quieres que quede en pantalla
            if (videoRawImage != null)
            {
                videoRawImage.gameObject.SetActive(false);
            }
        }
        else
        {
            // Si por alg�n motivo no hay VideoPlayer, hacemos un delay "de cortes�a"
            yield return new WaitForSeconds(delayBeforeSceneLoad);
        }

        // Finalmente, cargamos la siguiente escena
        SceneManager.LoadScene(index);
    }
}
