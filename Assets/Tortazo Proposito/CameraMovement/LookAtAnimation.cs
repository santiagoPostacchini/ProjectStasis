using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAnimation : MonoBehaviour
{
    public CameraController2 playerController;
    public CameraLookAt cameraLookAt;
    [SerializeField] GameObject targetObj, objectToLook;


    public void RotateCameraPlayer()
    {
        StartCoroutine(LookAtObject());
    }
    IEnumerator LookAtObject()
    {
        cameraLookAt.lookTarget = objectToLook.transform;
        cameraLookAt.LookAtTarget(); // <--- esto es lo que te faltaba
        playerController.canRotateCamera = false;
        cameraLookAt.canLook = true;

        yield return new WaitForSeconds(4f);

        cameraLookAt.canLook = false;
        playerController.canRotateCamera = true;
    }
}
