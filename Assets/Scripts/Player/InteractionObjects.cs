using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using NuevoInteractor;

public class InteractionObjects : MonoBehaviour
{
    private StasisObjectEffects stasisObjectEffects;
    private void Start()
    {
        stasisObjectEffects = GetComponentInChildren<StasisObjectEffects>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DetectInteractableObjects();
        }
        DetectPhysicsObjects();
    }
    
    private void DetectInteractableObjects()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 4))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
    private void DetectPhysicsObjects()
    {
        bool a = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit1, 100);
        PhysicsObject physicsObject = hit1.collider.GetComponent<PhysicsObject>();
        if (physicsObject != null)
        {
            stasisObjectEffects?.HandleVisualStasisFeedback(physicsObject, true);
        }
        else
        {
            stasisObjectEffects?.HandleVisualStasisFeedback(physicsObject, false);
        }
    }

}
