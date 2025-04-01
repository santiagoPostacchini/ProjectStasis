using UnityEngine;

public class PhysicsBox : PhysicsObject, IStasis
{
    [SerializeField]private bool canDrop;
    public override void Grab(Transform objGrabPointTransform)
    {
        if (!canDrop) return;
        StartCoroutine(EnableCollisionCheckAfterDelay(collisionCheckDelay));
        this.objGrabPointTransform = objGrabPointTransform;
        objRB.useGravity = false;
        objRB.drag = 10;
    }

    public override void Drop()
    {
        if (!canDrop) return;
        this.objGrabPointTransform = null;
        objRB.useGravity = true;
        objRB.drag = 1;
    }

    public override void Throw(Transform objGrabPointTransform, float force)
    {
        if (!canDrop) return;
        Drop();
        Vector3 throwVelocity = objGrabPointTransform.forward * (force / objRB.mass);
        objRB.AddForce(throwVelocity);
    }

    private void FixedUpdate()
    {
        if (objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.transform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.localRotation, objGrabPointTransform.transform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MoveRotation(newRot);
            objRB.MovePosition(newPos);
        }
    }

    public void StatisEffectActivate()
    {
        FreezeObject();
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }
}
