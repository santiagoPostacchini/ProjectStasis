using System.Collections;
using System.Collections.Generic;
using Puzzle_Elements;
using UnityEngine;

public class ButtonStasis : PhysicsObject, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;
    [SerializeField]private PressurePlate pressurePlate;

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
            if (pressurePlate != null) pressurePlate.isFrozen = true;
            SaveRigidbodyState();
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            objRB.useGravity = false;
            _isFreezed = true;
            SetColorOutline(Color.green, 0.75f);
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
            if (pressurePlate != null) pressurePlate.isFrozen = false;
            RestoreRigidbodyState();
            _isFreezed = false;
            objRB.useGravity = true;
            SetOutlineThickness(1f); // Reset visual cue.
        }
    }
    
}
