using UnityEngine;
using Events;

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
        [SerializeField] private Player player; // ← Referencia al jugador
        private Vector3 _startPos;

        private bool _footstepTriggeredThisCycle;
        private float _previousBobbingValue;

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
            
            float bobbingValue = Mathf.Sin(Time.time * frecuency);
            
            pos.y = bobbingValue * amplitude;
            pos.x = Mathf.Sin(Time.time * frecuency / 2) * amplitude / 2;

            CheckFootstepEvent(bobbingValue); // Método modificado

            _previousBobbingValue = bobbingValue;

            return pos;
        }

        private void CheckMotion()
        {
            float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

            if (speed < _toggleSpeed)
            {
                _footstepTriggeredThisCycle = false;
                return;
            }

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

        private void CheckFootstepEvent(float currentBobbingValue)
        {
            if (_previousBobbingValue < 0 && currentBobbingValue >= 0)
            {
                if (!_footstepTriggeredThisCycle && player && player.state != Player.MovementState.Air)
                {
                    EventManager.TriggerEvent("OnFootstep", player.gameObject);
                    _footstepTriggeredThisCycle = true;
                }
            }

            if (currentBobbingValue < 0)
                _footstepTriggeredThisCycle = false;
        }
    }
}