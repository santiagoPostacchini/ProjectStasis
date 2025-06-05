using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezablePlatform : PhysicsObject, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;
    [SerializeField] private MovingPlatform movingPlatform;

    public bool IsFreezed => _isFreezed;

    private void Awake()
    {
        movingPlatform = GetComponent<MovingPlatform>();
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
            if (movingPlatform != null)
            {
                movingPlatform.canMove = false;
            }
            else
            {
                Debug.LogWarning("movingPlatform es null en FreezeObject, pero debería estar asignado");
            }

            SaveRigidbodyState();
            objRB.isKinematic = false;
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            _isFreezed = true;
            SetColorOutline(Color.green, 1);
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
            movingPlatform.canMove = true;
            objRB.isKinematic = false;
            SetOutlineThickness(1f); // Reset visual cue.
            //
        }
    }

    
}
