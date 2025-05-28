using System.Collections;
using Audio;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(LineRenderer))]
    public class StasisBeam : MonoBehaviour
    {
        [SerializeField] private float moveDuration = .3f;
        [SerializeField] private Light lightStasis;

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
            if (!hit)
            {
                AudioManager.Instance.PlaySfxOnObject("StasisFail", this.GetComponent<AudioSource>());
            }
            else
            {
                AudioManager.Instance.PlaySfxOnObject("StasisSuccess", this.GetComponent<AudioSource>());
            }
            
            yield return new WaitForSeconds(.3f);
            DisableBeam();
        }
        
        private void DisableBeam()
        {
            gameObject.SetActive(false);
        }
    }
}
