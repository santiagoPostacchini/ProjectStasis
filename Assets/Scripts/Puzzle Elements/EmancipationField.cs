using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmancipationField : MonoBehaviour
{
    [SerializeField] private Transform t;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
       // PhysicsObject physicsObject = other.GetComponent<PhysicsObject>();

        if (player == null) Debug.Log("PLAYER NULL");
       // if (physicsObject == null) Debug.Log("physicsObject NULL");
        if (player != null)
        {
            if (player.playerInteractor._objectGrabbable == null) return;
            PhysicsObject p = player.playerInteractor._objectGrabbable;
            PhysicsBox box = p as PhysicsBox;
            if (box != null && box.IsFreezed) box.UnfreezeObject();
            player.playerInteractor._objectGrabbable.Drop();
            player.playerInteractor._objectGrabbable.gameObject.transform.position = t.position;
            player.playerInteractor.ClearHands();
        }

    }
}
