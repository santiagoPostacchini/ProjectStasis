using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [SerializeField] private bool _isEnabled = true;
    [SerializeField, Range(0, 0.1f)] private float _amplitude;
    [SerializeField, Range(0, 30)] private float _frecuency;

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _cameraHolder;

    private float _toggleSpeed = 1f;

    private Rigidbody _rb;
    private Vector3 _startPos;

    private void Awake()
    { 
        _rb = GetComponent<Rigidbody>();
        _startPos = _camera.localPosition;
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
        pos.y = Mathf.Sin(Time.time * _frecuency) * _amplitude;
        pos.x = Mathf.Sin(Time.time * _frecuency / 2) * _amplitude / 2;
        return pos;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;

        if (speed < _toggleSpeed) return;

        PlayMotion(FootstepMotion());
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
}
