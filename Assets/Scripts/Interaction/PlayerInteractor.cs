using System.Collections;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _throwForce;
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private Transform _objectGrabPointTransform;

     public PhysicsObject _objectGrabbable;
    private bool _isInteractableInView = false;
    [SerializeField] private Player.Player player;

    private float timer;
    private bool isPressingE = false;
    private bool hasThrown = false;
    private bool canFill = true;

    [SerializeField] private FillAnimation _fillAnimation;
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
            timer = 0f;
            isPressingE = true;
            hasThrown = false;
            canFill = true;
            _fillAnimation.ResetBrokenFill();
            if (_objectGrabbable != null && isPressingE && canFill)
            {
                _fillAnimation.ActivateGameObject();   // Mostrar animación de carga
                _fillAnimation.IsPressed(true);        // Inicia la animación visual del fill
            }
            
        }

        if (isPressingE && Input.GetKey(KeyCode.E))
        {
            timer += Time.deltaTime;

            if (!hasThrown && timer >= 1f)
            {
                if (_objectGrabbable != null)
                {
                    _objectGrabbable.Throw(_objectGrabPointTransform, _throwForce);

                    _fillAnimation.BrokenFill(true);     // Animación de carga completada (rotura)
                    canFill = false;
                    StartCoroutine(DelayedClearHands());
                    hasThrown = true;
                    isPressingE = false; // Evita que vuelva a lanzar mientras seguís presionando
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            _fillAnimation.IsPressed(false); // Siempre se llama, pero internamente decide qué mostrar
            timer = 0f;
            isPressingE = false;

           

            if (!hasThrown)
            {
                HandleGrabOrDrop(hitObject);
            }

            hasThrown = false;
            canFill = false;
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
                _objectGrabbable.player = player;
            }
        }
        else
        {
            _objectGrabbable.Drop();
            _objectGrabbable.player = null;
            ClearHands();
        }
    }

    public void ClearHands()
    {
        _objectGrabbable = null;
    }

    public bool HasObjectInHand()
    {
        return _objectGrabbable != null;
    }
}
