using UnityEngine;

public class PhysicsBox : PhysicsObject
{
    private void Awake()
    {
        objRB = GetComponent<Rigidbody>();
        collisionDetector = objRB.GetComponent<CollisionDetector>();
    }

    public override void Freeze()
    {
        Debug.Log("Freezed set to " + _isFreezed);
    }

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
        objRB.useGravity = true;
        objRB.drag = 1;
    }

    public override void Throw(Transform objGrabPointTransform, float force)
    {
        Drop();
        objRB.AddForce(objGrabPointTransform.forward * (force/objRB.mass));
    }

    private void FixedUpdate()
    {
        if(objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.transform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.localRotation, objGrabPointTransform.transform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MoveRotation(newRot);
            objRB.MovePosition(newPos);
        }
    }
}