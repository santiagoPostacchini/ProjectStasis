using System.Collections;
using Events;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class StasisBeam : MonoBehaviour
    {
        [SerializeField] private float moveDuration = .3f;
        [SerializeField] private Light lightStasis;

        // Nombres de evento para éxito y fallo
        [Header("Eventos de Sonido")]
        [Tooltip("Evento que se dispara si no golpea un objeto staseable")]
        public string failEventName = "StasisFail";
        [Tooltip("Evento que se dispara si golpea un objeto staseable")]
        public string successEventName = "StasisSuccess";

        public void SetBeam(Vector3 start, Vector3 end, bool hit)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            transform.position = start;
            StartCoroutine(MoveBeam(start, end, hit));
        }

        private IEnumerator MoveBeam(Vector3 start, Vector3 end, bool hit)
        {
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                transform.position = Vector3.Lerp(start, end, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            lightStasis.enabled = false;

            // Disparo de eventos en lugar de AudioManager
            EventManager.TriggerEvent(hit ? successEventName : failEventName, gameObject);

            yield return new WaitForSeconds(.3f);
            DisableBeam();
        }

        private void DisableBeam()
        {
            gameObject.SetActive(false);
        }
    }
}