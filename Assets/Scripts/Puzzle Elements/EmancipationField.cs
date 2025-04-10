using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmancipationField : MonoBehaviour
{
    [SerializeField] private Transform t;
    
    
    private void OnTriggerEnter(Collider other)
    {
        PhysicsObject physicObject = other.GetComponent<PhysicsObject>();
        if (physicObject != null)
        {
            Debug.Log("A");
            if (physicObject.player.playerInteractor._objectGrabbable == null) return;
            PhysicsBox box = physicObject as PhysicsBox;
            if (box != null && box.IsFreezed) box.UnfreezeObject();
            physicObject.player.playerInteractor._objectGrabbable.Drop();
            physicObject.player.playerInteractor._objectGrabbable.gameObject.transform.position = t.position;
            physicObject.player.playerInteractor.ClearHands();

        }
    }
}
