using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _pickUpRange = 3f;
    [SerializeField] private float _throwForce = 5f;
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private Transform _objectGrabPointTransform;

    private PhysicsObject _objectGrabbable;
    private bool _isInteractableInView = false;

    void Update()
    {
        bool hitInteractable = false;
        if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out RaycastHit hit, _pickUpRange))
        {
            if (hit.collider.gameObject.CompareTag("Interactable"))
            {
                hitInteractable = true;
            }
        }

        if (hitInteractable != _isInteractableInView && !_objectGrabbable)
        {
            _isInteractableInView = hitInteractable;
            CrosshairController.Instance.ToggleCrosshairSize(_isInteractableInView);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_objectGrabbable)
            {
                if (hitInteractable && hit.transform.TryGetComponent<PhysicsObject>(out _objectGrabbable))
                {
                    _objectGrabbable.Grab(_objectGrabPointTransform);
                }
            }
            else
            {
                _objectGrabbable.Drop();
                ClearHands();
            }
        }

        if (Input.GetMouseButtonDown(0) && _objectGrabbable)
        {
            _objectGrabbable.Throw(_objectGrabPointTransform, _throwForce);
            ClearHands();
        }
    }

    void ClearHands()
    {
        _objectGrabbable = null;
    }
}
