using UnityEngine;

namespace Player
{
    public class CameraBobbing : MonoBehaviour
    {
        private bool _isEnabled = true;
        [SerializeField, Range(0, 0.1f)] private float amplitude;
        [SerializeField, Range(0, 30)] private float frecuency;

        [SerializeField] private new Transform camera;
        [SerializeField] private Transform cameraHolder;

        private readonly float _toggleSpeed = 1f;

        [SerializeField] private CharacterController characterController;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = camera.localPosition;
        }

        private void Update()
        {
            if(!_isEnabled) return;

            CheckMotion();
            ResetPosition();
        }

        private Vector3 FootstepMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y = Mathf.Sin(Time.time * frecuency) * amplitude;
            pos.x = Mathf.Sin(Time.time * frecuency / 2) * amplitude / 2;
            return pos;
        }

        private void CheckMotion()
        {
            float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

            if (speed < _toggleSpeed) return;

            PlayMotion(FootstepMotion());
        }

        private void ResetPosition()
        {
            if (camera.localPosition == _startPos) return;
            camera.localPosition = Vector3.Lerp(camera.localPosition, _startPos, 1 * Time.deltaTime);
        }

        private void PlayMotion(Vector3 motion)
        {
            camera.localPosition += motion;
        }
    }
}
