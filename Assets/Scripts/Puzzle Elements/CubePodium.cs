using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using NuevoInteractor;

public class CubePodium : MonoBehaviour, IInteractable
{
    [SerializeField] private NewPhysicsBox box;
    [SerializeField] private Transform t;
    [SerializeField] private ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        SuspendObject();
        particle.Play();
    }
    private void Update()
    {
        if (box == null) return;
        float distance = Vector3.Distance(transform.position, box.transform.position);
        if (distance > 2)
        {
            Interact();
        }
    }
    private void SuspendObject()
    {
        if (box == null) return;
        Rigidbody rb = box.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            box.transform.position = t.transform.position;
        }

    }

    public void Interact()
    {
        particle?.Stop();

    }
}
