using UnityEngine;

public class MovableTrampoline : PhysicsObject, IStasis
{
    [Header("Trampoline Settings")]
    [Tooltip("Multiplier for the bounce speed (set to 1 to exactly return to the same height).")]
    public float bounceHeightMultiplier = 1f;

    [Tooltip("Threshold fall speed that distinguishes a significant fall from a light step.")]
    public float fallSpeedThreshold = 1f;

    [Tooltip("Minimal bounce speed when the object is not falling fast (e.g. when just walking on the trampoline).")]
    public float minimalBounceSpeed = 1f;

    // Stasis-related variables
    private Vector3 freezePosition;
    private Quaternion freezeRotation;
    private bool _isFreezed;

    /// <summary>
    /// When an object collides, determine a realistic bounce direction using the contact normal.
    /// Then compute the bounce speed based on the object's falling speed, using collision.relativeVelocity
    /// to capture the full impact speed.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRB = collision.rigidbody;
        if (otherRB != null && collision.contactCount > 0)
        {
            // Get the contact normal from the first contact point.
            Vector3 contactNormal = collision.contacts[0].normal;

            // Use the collision's relative velocity instead of the current Rigidbody velocity.
            Vector3 incomingVelocity = collision.relativeVelocity;

            // Compute the fall speed (the magnitude of the downward component) using global up.
            float fallSpeed = Mathf.Max(0, -Vector3.Dot(incomingVelocity, Vector3.up));

            // Determine bounce speed:
            // - If the fall speed is above the threshold, use that value.
            // - Otherwise, use the minimal bounce speed.
            float bounceSpeed = (fallSpeed < fallSpeedThreshold) ? minimalBounceSpeed : fallSpeed;
            bounceSpeed *= bounceHeightMultiplier;

            // Reflect the incoming velocity using the contact normal.
            Vector3 reflectedDir = Vector3.Reflect(incomingVelocity, contactNormal).normalized;
            // Ensure the reflected direction is upward; if not, default to global up.
            if (reflectedDir.y <= 0)
            {
                reflectedDir = Vector3.up;
            }

            // Set the object's new velocity along the computed direction with the determined magnitude.
            otherRB.velocity = reflectedDir * bounceSpeed;
        }
    }

    /// <summary>
    /// FixedUpdate handles movement when the trampoline is grabbed or frozen.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.rotation, objGrabPointTransform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MovePosition(newPos);
            objRB.MoveRotation(newRot);
        }
        else if (_isFreezed)
        {
            objRB.MovePosition(freezePosition);
            objRB.MoveRotation(freezeRotation);
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Activates the stasis effect by freezing the trampoline in place.
    /// </summary>
    public void StatisEffectActivate()
    {
        FreezeObject();
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
    }

    /// <summary>
    /// Deactivates the stasis effect.
    /// </summary>
    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    /// <summary>
    /// Freezes the trampoline by saving its state, stopping motion, and disabling gravity.
    /// </summary>
    private void FreezeObject()
    {
        if (!_isFreezed)
        {
            SaveRigidbodyState();
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            objRB.useGravity = false;
            _isFreezed = true;
            SetOutlineThickness(1.05f); // Visual cue for stasis.
        }
    }

    /// <summary>
    /// Unfreezes the trampoline by restoring its state and re-enabling gravity.
    /// </summary>
    private void UnfreezeObject()
    {
        if (_isFreezed)
        {
            RestoreRigidbodyState();
            _isFreezed = false;
            objRB.useGravity = true;
            SetOutlineThickness(1f); // Reset visual cue.
        }
    }

    public override void Drop()
    {
        base.Drop();
        if (_isFreezed)
        {
            freezePosition = transform.position;
            freezeRotation = transform.rotation;
        }
    }

    public override void Throw(Transform grabPoint, float force)
    {
        Drop();
        if (!_isFreezed)
        {
            Vector3 throwVelocity = grabPoint.forward * (force / objRB.mass);
            objRB.AddForce(throwVelocity);
        }
    }
}
