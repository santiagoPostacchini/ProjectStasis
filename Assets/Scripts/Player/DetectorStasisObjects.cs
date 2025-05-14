using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorStasisObjects : MonoBehaviour
{
    private bool isAimingAtFreezable;
    private PhysicsObject _physicObject;

    void Start()
    {
        isAimingAtFreezable = false;
    }

    void Update()
    {
        DetectStasisObjects(Camera.main.transform);
    }

    public void DetectStasisObjects(Transform playerCameraTransform)
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, 100))
        {
            PhysicsObject po = hit.collider.GetComponent<PhysicsObject>();

            if (po != null && po is IStasis stasis)
            {
                if (isAimingAtFreezable) return;
                isAimingAtFreezable = true;

                if (po._isFreezed) return;

                _physicObject = po;

                if (_physicObject.player != null)
                {
                    if (_physicObject.player.playerInteractor._objectGrabbable != null) return;
                }

                // Activar glow
                _physicObject.Glow(true, 2f);

                //  Reproducir sonido de selección de objeto stasis
                AudioManager.Instance?.PlaySfx("SelectStasiable");

                return;
            }
        }

        // Si no se apunta más a un objeto válido:
        isAimingAtFreezable = false;

        if (_physicObject != null)
        {
            _physicObject.Glow(false, 1);
            _physicObject = null;
        }
    }
}
