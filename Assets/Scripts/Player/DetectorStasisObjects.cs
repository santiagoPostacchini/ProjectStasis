using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorStasisObjects : MonoBehaviour
{

    private bool isAimingAtFreezable;
    private PhysicsObject _physicObject;
    // Start is called before the first frame update
    void Start()
    {
        isAimingAtFreezable = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetectStasisObjects(Camera.main.transform);
    }
    public void DetectStasisObjects(Transform playerCameraTransform)
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, 100))
        {
            
            PhysicsObject po = hit.collider.GetComponent<PhysicsObject>();
            if(po !=null && po is IStasis stasis)
            {
                if (isAimingAtFreezable) return;
                isAimingAtFreezable = true;
                if (po._isFreezed) return;

                _physicObject = po;
                if(_physicObject.player != null)
                {
                    if (_physicObject.player.playerInteractor._objectGrabbable != null) return;
                }
                
                _physicObject.Glow(true,2f);
                
               
            }
            else
            {
                isAimingAtFreezable = false;

                if (_physicObject != null)
                {
                    _physicObject.Glow(false, 1);

                    _physicObject = null;
                } 
                
            }
        }
    }
    
}
