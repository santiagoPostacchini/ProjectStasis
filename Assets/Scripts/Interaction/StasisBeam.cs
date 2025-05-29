using System.Collections;
using Events;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class StasisBeam : MonoBehaviour
    {
        [Header("Movimiento")]
        [Tooltip("Velocidad a la que avanza el rayo (unidades/segundo)")]
        [SerializeField] private float beamSpeed = 30f;
        [Tooltip("Tiempo que mantiene encendida la luz tras detenerse")]
        [SerializeField] private float lightOffDelay = .3f;
        [SerializeField] private Light lightStasis;

        [Header("Eventos de Sonido")]
        [Tooltip("Evento que se dispara si no golpea un objeto staseable")]
        public string failEventName    = "StasisFail";
        [Tooltip("Evento que se dispara si golpea un objeto staseable")]
        public string successEventName = "StasisSuccess";

        private Coroutine _beamRoutine;
        
        public void SetBeam(Vector3 start, Vector3 end, bool hit)
        {
            // Interrumpe cualquier rayo previo
            if (_beamRoutine != null) 
                StopCoroutine(_beamRoutine);

            transform.position = start;
            // Calcula la dirección y distancia
            Vector3 direction = (end - start).normalized;
            float distance    = Vector3.Distance(start, end);
            if(lightStasis != null)
            {
                lightStasis.enabled = true;
            }
           
            gameObject.SetActive(true);

            _beamRoutine = StartCoroutine(MoveForward(direction, distance, hit));
        }

        private IEnumerator MoveForward(Vector3 direction, float maxDistance, bool hit)
        {
            float travelled = 0f;

            while (travelled < maxDistance)
            {
                float step = beamSpeed * Time.deltaTime;
                
                // Comprueba colisión en este frame
                if (Physics.Raycast(transform.position, direction, out var info, step))
                {
                    // Detén el rayo en el punto de impacto
                    transform.position = info.point;
                    break;
                }

                // Avanza
                transform.position += direction * step;
                travelled           += step;
                yield return null;
            }

            // Apago la luz
            if (lightStasis != null)
            {
                lightStasis.enabled = false;
            }

            // Disparo el evento correspondiente según el bool 'hit'
            EventManager.TriggerEvent(hit ? successEventName : failEventName, gameObject);

            // Espero un poquito antes de ocultar el rayo
            yield return new WaitForSeconds(lightOffDelay);
            DisableBeam();
        }

        private void DisableBeam()
        {
            gameObject.SetActive(false);
        }
    }
}
