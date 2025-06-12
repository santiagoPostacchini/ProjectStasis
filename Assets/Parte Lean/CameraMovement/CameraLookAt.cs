using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform lookTarget;

    public float lookSpeed = 6;
    public float lookAtTargetSpeed = 3;
    public float lookAtTimeDuration = 2;

    private Vector2 currentLookRotation = Vector2.zero;
    private Transform t;
    public bool isLookingAtTarget = false;
    private float lookAtTimeRemaining = 0;
    private Quaternion originalRotation;
    public bool canLook= false;

    private void Start()
    {
        t = transform;
        originalRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        LookControls();
        UpdateLookAtTarget();
    }

    void UpdateLookAtTarget()
    {
        if (!canLook) return;
        if (!isLookingAtTarget) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget.position - transform.position, Vector3.up), lookAtTargetSpeed * Time.deltaTime);
        if(lookAtTimeRemaining > 0)
        {
            lookAtTimeRemaining -= Time.deltaTime;
            return;
        }
        LookAtTarget();
        StopLookAtTarget();
    }
    private void StopLookAtTarget()
    {
        lookAtTimeRemaining = 0;
        isLookingAtTarget = false;
        

        float y = transform.eulerAngles.y;
        currentLookRotation.y = y;

        float x = transform.eulerAngles.x;
        if (x - 360 > -90) x -= 360;
        currentLookRotation.x = x;
    }

    public void LookAtTarget()
    {
        isLookingAtTarget = true;
        lookAtTimeRemaining = lookAtTimeDuration;
    }
    void LateUpdate()
    {
        ApplyRotation();
    }
    void ApplyRotation()
    {
        if (!canLook) return;
        if (isLookingAtTarget)
            return;

        Quaternion xQuaternion = Quaternion.AngleAxis(currentLookRotation.y, Vector3.up);

        Quaternion yQuaternion = Quaternion.AngleAxis(currentLookRotation.x, Vector3.right);
        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    public void LookControls()
    {
        /*
        if (isLookingAtTarget)
            return;
	    
        if (Input.GetKeyDown(KeyCode.Space) && lookTarget != null)
        {
            LookAtTarget();
            return;
        }
		*/
        if (!canLook)
        {
            return;
        }

        //currentLookRotation.y += Input.GetAxis("Mouse X") * lookSpeed * 360 * Time.deltaTime;
        currentLookRotation.y += Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        //currentLookRotation.x += -Input.GetAxis("Mouse Y") * lookSpeed * 360 * Time.deltaTime;
        currentLookRotation.x += -Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;

        currentLookRotation.x = Mathf.Clamp(currentLookRotation.x, -90, 90);
    }
}
