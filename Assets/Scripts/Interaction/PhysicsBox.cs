using UnityEngine;

public class PhysicsBox : PhysicsObject, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;

    public override void Grab(Transform objGrabPointTransform)
    {
        StartCoroutine(EnableCollisionCheckAfterDelay(collisionCheckDelay));
        this.objGrabPointTransform = objGrabPointTransform;
        objRB.useGravity = false;
        objRB.drag = 10;
    }

    public override void Drop()
    {
        this.objGrabPointTransform = null;
        if (_isFreezed)
        {
            freezePosition = transform.position;
            freezeRotation = transform.rotation;
        }
        else
        {
            objRB.useGravity = true;
            objRB.drag = 1;
        }
    }

    public override void Throw(Transform objGrabPointTransform, float force)
    {
        Drop();
        if (!_isFreezed)
        {
            Vector3 throwVelocity = objGrabPointTransform.forward * (force / objRB.mass);
            objRB.AddForce(throwVelocity);
        }
    }

    private void FixedUpdate()
    {
        if (objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.rotation, objGrabPointTransform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MoveRotation(newRot);
            objRB.MovePosition(newPos);
        }
        else if (_isFreezed)
        {
            objRB.MovePosition(freezePosition);
            objRB.MoveRotation(freezeRotation);
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    public void StatisEffectActivate()
    {
        FreezeObject();
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }
}
