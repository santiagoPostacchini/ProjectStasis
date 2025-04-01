using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour,IStasis
{
    public Material matStasis;
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;
    private string OutlineThicknessName = "_BorderThickness";


    [SerializeField] private float activationMassThreshold = 10f;
    [SerializeField] private PressurePlateGroup plateGroup;
    [SerializeField] private Animator animator;
    [SerializeField] private bool isFrozen;
    [SerializeField]private bool isButtonPressed;
   
    public bool isActivated { get; private set; } = false;
    private readonly List<Rigidbody> objectsOnPlate = new();

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && !objectsOnPlate.Contains(rb))
        {
            objectsOnPlate.Add(rb);
            UpdateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && objectsOnPlate.Contains(rb))
        {
            objectsOnPlate.Remove(rb);
            UpdateState();
        }
    }

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
            animator?.SetBool("IsPressed", isActivated);
            //isButtonPressed = isActivated;
            plateGroup?.NotifyPlateStateChanged();
        }
    }

    public void StatisEffectActivate()
    {
        StateStasisEffect(1.2f);
    }

    public void StatisEffectDeactivate()
    {
        StateStasisEffect(1f);
    }
    public void StateStasisEffect(float f)
    {
        isFrozen = !isFrozen;
        SetOutlineThickness(f);
        UpdateState();
    }
    private void SetOutlineThickness(float thickness)
    {
        if (_renderer != null && _mpb != null)
        {
            Debug.Log("Bordes");
            _renderer.GetPropertyBlock(_mpb);  // Get the current property block
            _mpb.SetFloat(OutlineThicknessName, thickness);  // Set the thickness value
            _renderer.SetPropertyBlock(_mpb);  // Apply the updated property block
        }
    }
}
