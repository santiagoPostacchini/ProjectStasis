using UnityEngine;

public class ObjectUpdater : MonoBehaviour
{
    [SerializeField] private int _heightDifference;
    [SerializeField] private Transform _objectFuture;
    private Rigidbody _rb;
    private bool hasUpdatedFuture = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_rb.velocity != Vector3.zero)
        {
            hasUpdatedFuture = false;
            
        }
        else
        {
            if (!hasUpdatedFuture)
            {
                UpdateObjectFuture();
                hasUpdatedFuture = true;
            }
        }
    }

    public void UpdateObjectFuture()
    {
        PhysicsObject physObj = GetComponent<PhysicsObject>();
        if (physObj != null && physObj.IsGrabbed)
        {
            physObj.Drop();

            PlayerInteractor interactor = physObj.GetComponentInParent<PlayerInteractor>();
            if (interactor != null)
            {
                interactor.ClearHands();
            }
        }

        _objectFuture.position = new Vector3(transform.position.x,
                                               transform.position.y + _heightDifference,
                                               transform.position.z);
        _objectFuture.rotation = transform.rotation;
    }
}
