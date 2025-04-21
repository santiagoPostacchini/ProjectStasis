using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private float activationMassThreshold = 10f;
    [SerializeField] private PressurePlateGroup plateGroup;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particles;
    public bool isFrozen;
    
    public bool isActivated { get; private set; } = false;
    private readonly List<Rigidbody> objectsOnPlate = new();

    private void OnCollisionEnter(Collision collision)
    {
        var rb = collision.rigidbody;
        if (rb != null && !objectsOnPlate.Contains(rb))
        {
           
            objectsOnPlate.Add(rb);
            UpdateState();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        var rb = collision.rigidbody;
        if (rb != null && objectsOnPlate.Contains(rb))
        {
            Debug.Log("Saliendo");
            objectsOnPlate.Remove(rb);
            UpdateState();
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    var rb = other.attachedRigidbody;
    //    if (rb != null && !objectsOnPlate.Contains(rb))
    //    {
    //        objectsOnPlate.Remove(rb);
    //        UpdateState();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    var rb = other.attachedRigidbody;
    //    if (rb != null && objectsOnPlate.Contains(rb))
    //    {
    //        objectsOnPlate.Remove(rb);
    //        UpdateState();
    //    }
    //}

    private void UpdateState()
    {
        if (isFrozen) return;
        bool heavyEnoughObjectFound = false;

        foreach (var rb in objectsOnPlate)
        {
            if (rb != null && rb.mass >= activationMassThreshold)
            {
                heavyEnoughObjectFound = true;
                break;
            }
        }
        if (heavyEnoughObjectFound != isActivated)
        {
            isActivated = heavyEnoughObjectFound;
            if(plateGroup != null)
            {
                animator?.SetBool("IsPressed", isActivated);
                plateGroup?.NotifyPlateStateChanged();
            }
          
        }
    }

    public void ActivateEffectParticle()
    {
        if (particles == null) return;
        particles.Play();
    }
    public void DesactivateEffectParticle()
    {
        if (particles == null) return;
        particles.Stop();
    }
}
