using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionDetector))]
public abstract class PhysicsObject : MonoBehaviour
{
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

    public abstract void Freeze();
    public abstract void Grab(Transform objGrabPointTransform);
    public abstract void Drop();
    public abstract void Throw(Transform objGrabPointTransform, float force);

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
                IsUnderHighPressure = true;
                Drop();
                StartCoroutine(ResetForcesAfterDrop());
            }
            else
            {
                IsUnderHighPressure = false;
            }

            //Debug.Log("Average pressure over " + accumulationTime + " seconds: " + averageImpulse + " on " + this.name);

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
}