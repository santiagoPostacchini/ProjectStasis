using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float pickUpRange = 3f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;

    private PhysicsObject objectGrabbable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!objectGrabbable)
            {
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, pickUpRange))
                {
                    if (hit.transform.TryGetComponent<PhysicsObject>(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            }
            else
            {
                objectGrabbable.Drop();
                ClearHands();

            }
        }
        if (Input.GetMouseButtonDown(0) && objectGrabbable)
        {
            objectGrabbable.Throw(objectGrabPointTransform, throwForce);
            ClearHands();
        }
    }

    void ClearHands()
    {
        objectGrabbable = null;
    }
}