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

                //_physicObject.Glow(true,2f);
                Color softGreen = new Color(0.7f, 1f, 0.7f, 1f);
                _physicObject.SetColorOutline(softGreen, 0.3f);
                _physicObject.SetOutlineThickness(1.05f);
               


            }
            else
            {
                isAimingAtFreezable = false;

                if (_physicObject != null)
                {
                    if (!_physicObject._isFreezed)
                    {
                        _physicObject.SetColorOutline(Color.green, 1);
                        _physicObject.SetOutlineThickness(1f);
                    }
                    
                   // _physicObject.Glow(false, 1);

                    _physicObject = null;
                } 
                
            }
        }
    }
    
}
