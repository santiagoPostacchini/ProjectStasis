using System.Collections;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _throwForce;
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private Transform _objectGrabPointTransform;

    private PhysicsObject _objectGrabbable;
    private bool _isInteractableInView = false;

    void Update()
    {
        bool hitInteractable = false;
        GameObject hitObject = null;

        if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out RaycastHit hit, _pickUpRange))
        {
            hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Interactable"))
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
            HandleGrabOrDrop(hitObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_objectGrabbable != null)
            {
                _objectGrabbable.Throw(_objectGrabPointTransform, _throwForce);

                StartCoroutine(DelayedClearHands());
            }
        }
    }

    private IEnumerator DelayedClearHands()
    {
        yield return new WaitForEndOfFrame();
        ClearHands();
    }

    void HandleGrabOrDrop(GameObject hitObject)
    {
        if (_objectGrabbable == null)
        {
            if (hitObject != null && hitObject.TryGetComponent(out _objectGrabbable))
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

    void ClearHands()
    {
        _objectGrabbable = null;
    }

    public bool HasObjectInHand()
    {
        return _objectGrabbable != null;
    }
}
