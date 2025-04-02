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

    private float accumulatedImpulse = 0f;
    private float accumulationTime = 0f;
    private float accumulationPeriod = 0.1f;

    private bool _canCheckCollisions = false;

    protected CollisionDetector collisionDetector;
    protected Rigidbody objRB;
    protected Transform objGrabPointTransform;

    private Vector3 _savedVelocity;
    private Vector3 _savedAngularVelocity;
    private float _savedDrag;

    public abstract void Grab(Transform objGrabPointTransform);
    public abstract void Drop();
    public abstract void Throw(Transform objGrabPointTransform, float force);

    private void Start()
    {
        objRB = GetComponent<Rigidbody>();
        collisionDetector = GetComponent<CollisionDetector>();

        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        SetOutlineThickness(1f);
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
                if (!_isFreezed)
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

    public void SaveRigidbodyState()
    {
        if (objRB != null)
        {
            _savedVelocity = objRB.velocity;
            _savedAngularVelocity = objRB.angularVelocity;
            _savedDrag = objRB.drag;
        }
    }

    public void RestoreRigidbodyState()
    {
        if (objRB != null)
        {
            objRB.velocity = _savedVelocity;
            objRB.angularVelocity = _savedAngularVelocity;
            objRB.drag = _savedDrag;
            objRB.WakeUp();
        }
    }

    public void FreezeObject()
    {
        if (!_isFreezed)
        {
            SaveRigidbodyState();
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            objRB.useGravity = false;
            _isFreezed = true;

            SetOutlineThickness(1.05f);
        }
    }

    public void UnfreezeObject()
    {
        if (_isFreezed)
        {
            RestoreRigidbodyState();
            _isFreezed = false;
            objRB.useGravity = true;
            SetOutlineThickness(1f);
        }
    }

    private void SetOutlineThickness(float thickness)
    {
        if (_renderer != null && _mpb != null)
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(OutlineThicknessName, thickness);
            _renderer.SetPropertyBlock(_mpb);
        }
    }

    public bool IsGrabbed
    {
        get { return objGrabPointTransform != null; }
    }

}
