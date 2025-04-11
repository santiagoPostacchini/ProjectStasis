using UnityEngine;
using System.Collections;

public class PhysicsBox : PhysicsObject, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;
    private bool _isFreezed;

    public bool IsFreezed
    {
        get => _isFreezed;
        private set => _isFreezed = value;
    }
    public override void Grab(Transform grabPoint)
    {
        base.Grab(grabPoint);
        // Additional stasis-related logic on grab can be added here if needed.
    }

    public override void Drop()
    {
        base.Drop();
        if (_isFreezed)
        {
            // Store the current transform values if the object is frozen.
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
            // Maintain the frozen position and rotation.
            objRB.MovePosition(freezePosition);
            objRB.MoveRotation(freezeRotation);
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    // Activates the stasis effect and freezes the object.
    public void StatisEffectActivate()
    {
        FreezeObject();
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
    }

    // Deactivates the stasis effect and unfreezes the object.
    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    private void FreezeObject()
    {
        if (!_isFreezed)
        {
            SaveRigidbodyState();
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            objRB.useGravity = false;
            _isFreezed = true;
            SetOutlineThickness(1.05f); // Optionally update the visual to indicate stasis.
        }
    }

    public void UnfreezeObject()
    {
        if (_isFreezed)
        {
            RestoreRigidbodyState();
            _isFreezed = false;
            if(objRB)
                objRB.useGravity = true;
            SetOutlineThickness(1f); // Reset the visual effect.
        }
    }
}
