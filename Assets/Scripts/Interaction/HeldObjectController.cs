using System.Collections;
using UnityEngine;

public class HeldObjectController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float alignDuration = 0.2f; // How fast to align on pickup.
    [SerializeField] private float scrollMoveSpeed = 3.5f; // Speed for mouse scroll adjustments.
    [SerializeField] private float retractForce = 3f;      // Force applied when releasing with right mouse.

    private Transform holdArea;
    private Transform holdGhost;
    private Camera playerCamera;
    private Rigidbody rb;
    // This stores the fixed rotation once aligned.
    private Quaternion fixedRotation;

    public void Initialize(Transform holdContainer, Transform ghost, Camera cam)
    {
        holdArea = holdContainer;
        holdGhost = ghost;
        playerCamera = cam;
        rb = GetComponent<Rigidbody>();

        // Disable gravity and increase drag for smooth control.
        rb.useGravity = false;
        rb.drag = 10;

        // Begin the smooth alignment routine.
        StartCoroutine(AlignToHoldArea());
    }

    private IEnumerator AlignToHoldArea()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < alignDuration)
        {
            elapsed += Time.deltaTime;
            // Lerp position toward the hold area.
            transform.position = Vector3.Lerp(startPos, holdArea.position, elapsed / alignDuration);
            // Lerp rotation: preserve the original X angle, but reset Y & Z.
            Quaternion targetRot = Quaternion.Euler(startRot.eulerAngles.x, 0f, 0f);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, elapsed / alignDuration);
            yield return null;
        }

        transform.position = holdArea.position;
        transform.rotation = Quaternion.Euler(startRot.eulerAngles.x, 0f, 0f);
        // Freeze rotation to avoid unwanted tilting.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        fixedRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        // Zero the velocity to avoid drifting.
        rb.velocity = Vector3.zero;

        // Smoothly move toward the ghost target position.
        float lerpFactor = Vector3.Distance(transform.position, holdGhost.position) * Time.smoothDeltaTime * 2f;
        transform.position = Vector3.Lerp(rb.position, holdGhost.position, lerpFactor);
    }

    private void Update()
    {
        // Adjust the distance to the hold area using the mouse scroll wheel.
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0f)
        {
            // Move along the camera's forward direction.
            Vector3 direction = scroll > 0 ? Vector3.forward : Vector3.back;
            transform.Translate(direction * scrollMoveSpeed * Time.deltaTime, playerCamera.transform);
            // Keep the ghost at the new position.
            holdGhost.position = transform.position;
        }

        // If the right mouse button is held, apply a retract force and release.
        if (Input.GetKey("mouse 1"))
        {
            rb.AddForce(-(holdArea.position - transform.position) * retractForce, ForceMode.Impulse);
            ReleaseAndCleanup();
        }

        // Rotate the hold container with "R" (if desired).
        if (Input.GetKey("r"))
        {
            float angleY = Input.GetAxis("Mouse X") * 4.0f;
            float angleX = Input.GetAxis("Mouse Y") * 4.0f;
            Vector3 currentPos = transform.position;
            holdArea.Rotate(new Vector3(angleX, -angleY, 0));
            transform.position = currentPos;
        }

        // Optionally, release the object when the left mouse button is released.
        if (holdArea.childCount >= 2 && Input.GetMouseButtonUp(0))
        {
            ReleaseAndCleanup();
        }
    }

    /// <summary>
    /// Releases the held object.
    /// </summary>
    public void ReleaseObject(float throwForce)
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        rb.drag = 1;
        // Apply throw force if specified.
        rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        transform.SetParent(null);
        Destroy(this);
    }

    void ReleaseAndCleanup()
    {
        // If no throw is intended, call ReleaseObject with zero force.
        ReleaseObject(0f);
    }

    // Optional collision handlers to temporarily free rotation.
    private void OnCollisionEnter(Collision collision)
    {
        rb.freezeRotation = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.freezeRotation = true;
    }
}
