using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

public class CubePodium : MonoBehaviour, IInteractable
{
    [SerializeField] private PhysicsBox box;
    [SerializeField] private Transform t;
    [SerializeField] private ParticleSystem particle;
    private PlayerInteractor playerInteractor;

    // Start is called before the first frame update
    void Start()
    {
        SuspendObject();
        particle.Play();
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, box.transform.position);
        if (distance > 2)
        {
            Interact();
        }
    }
    private void SuspendObject()
    {
        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("Suspendido");
            rb.useGravity = false;
            box.transform.position = t.transform.position;
        }

    }

    public void Interact()
    {
        Debug.Log("Cubo agarrado");
        box.objRB.useGravity = true;
        particle?.Stop();

    }
}
