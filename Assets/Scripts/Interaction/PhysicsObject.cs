using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionDetector))]
public abstract class PhysicsObject : MonoBehaviour
{
    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;

    public float collisionCheckDelay = 0.3f;
    [HideInInspector] public bool IsUnderHighPressure;
    [HideInInspector] public bool _isFreezed;
    [SerializeField] private float _rigidbodyPressureThreshold;
    [SerializeField] private float _nonRigidbodyPressureThreshold;
    public bool isFrozen = false;

    private float accumulatedImpulse = 0f;
    private float accumulationTime = 0f;
    private float accumulationPeriod = 0.1f;

    private bool _canCheckCollisions = false;

    protected CollisionDetector collisionDetector;
    protected Rigidbody objRB;
    protected Transform objGrabPointTransform;

    private Vector3 _savedVelocity;
    private Vector3 _savedAngularVelocity;
    private bool _wasKinematicBeforeFreeze;

    public abstract void Grab(Transform objGrabPointTransform);
    public abstract void Drop();
    public abstract void Throw(Transform objGrabPointTransform, float force);

    private void Start()
    {
        objRB = GetComponent<Rigidbody>();
        collisionDetector = GetComponent<CollisionDetector>();

        _renderer = GetComponent<Renderer>();  // Get Renderer reference
        _mpb = new MaterialPropertyBlock();  // Create a new Material Property Block
        SetOutlineThickness(1f);  // Initialize the default outline thickness
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_canCheckCollisions)
            return;

        float currentImpulseForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        accumulatedImpulse += currentImpulseForce;
        accumulationTime += Time.fixedDeltaTime;

        if (accumulationTime >= accumulationPeriod)
        {
            float averageImpulse = accumulatedImpulse / accumulationTime;

            float threshold = collisionDetector.IsHittingNOTRB ? _nonRigidbodyPressureThreshold : _rigidbodyPressureThreshold;
            if (averageImpulse > threshold)
            {
                if (!isFrozen)
                {
                    IsUnderHighPressure = true;
                    Drop();
                    StartCoroutine(ResetForcesAfterDrop());
                }
            }
            else
            {
                IsUnderHighPressure = false;
            }

            accumulatedImpulse = 0f;
            accumulationTime = 0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsUnderHighPressure = false;
        accumulatedImpulse = 0f;
        accumulationTime = 0f;
    }

    protected IEnumerator ResetForcesAfterDrop()
    {
        yield return new WaitForFixedUpdate();

        if (objRB != null)
        {
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    protected IEnumerator EnableCollisionCheckAfterDelay(float delay)
    {
        _canCheckCollisions = false;
        yield return new WaitForSeconds(delay);
        _canCheckCollisions = true;
    }

    // ------------------ STASIS MANAGEMENT ------------------

    // Save Rigidbody state before freezing
    public void SaveRigidbodyState()
    {
        if (objRB != null)
        {
            _savedVelocity = objRB.velocity;
            _savedAngularVelocity = objRB.angularVelocity;
            _wasKinematicBeforeFreeze = objRB.isKinematic;
        }
    }

    // Restore Rigidbody state after unfreezing
    public void RestoreRigidbodyState()
    {
        if (objRB != null)
        {
            objRB.isKinematic = _wasKinematicBeforeFreeze;

            // Only restore velocity and angular velocity if it was not kinematic before
            if (!_wasKinematicBeforeFreeze)
            {
                objRB.velocity = _savedVelocity;
                objRB.angularVelocity = _savedAngularVelocity;
            }

            objRB.WakeUp(); // Ensure physics resumes correctly
        }
    }

    // Freeze the object and make it kinematic
    public void FreezeObject()
    {
        if (!isFrozen)
        {
            SaveRigidbodyState(); // Save Rigidbody state before freezing
            objRB.isKinematic = true;
            objRB.Sleep(); // Prevent unnecessary physics calculations
            isFrozen = true;

            SetOutlineThickness(1.05f);  // Increase outline thickness when frozen
        }
    }

    // Unfreeze the object and restore previous state
    public void UnfreezeObject()
    {
        if (isFrozen)
        {
            RestoreRigidbodyState(); // Restore Rigidbody state after unfreezing
            isFrozen = false;

            SetOutlineThickness(1f);  // Reset outline thickness when unfrozen
        }
    }

    // ------------------ OUTLINE MANAGEMENT ------------------

    // Apply outline thickness using MaterialPropertyBlock
    private void SetOutlineThickness(float thickness)
    {
        if (_renderer != null && _mpb != null)
        {
            _renderer.GetPropertyBlock(_mpb);  // Get the current property block
            _mpb.SetFloat(OutlineThicknessName, thickness);  // Set the thickness value
            _renderer.SetPropertyBlock(_mpb);  // Apply the updated property block
        }
    }
}
