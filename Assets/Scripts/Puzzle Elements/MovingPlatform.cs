using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;
    [HideInInspector] public bool canMove;
    public GameObject objectTransport;
    public Transform pos;

    private Rigidbody rb;
    private Vector3 target;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float pauseDuration = 0.5f;

    private Coroutine moveCoroutine;

    private Vector3 lastPosition;
    private Vector3 platformVelocity;

    private Rigidbody playerRbOnPlatform;

    private FreezablePlatform freezeablePlatform;

    private void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        target = pointB.position;
        lastPosition = rb.position;
        moveCoroutine = StartCoroutine(MovePlatform());
        freezeablePlatform = GetComponent<FreezablePlatform>();
    }

    private IEnumerator MovePlatform()
    {
        float currentSpeed = 0f;
        float accelerationTime = 0.5f;
        
        while (true)
        {

            
            while (Vector3.Distance(rb.position, target) > 0.05f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, speed, speed / accelerationTime * Time.fixedDeltaTime);
                Vector3 newPosition = Vector3.MoveTowards(rb.position, target, currentSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPosition);
                yield return new WaitForFixedUpdate();
            }

            currentSpeed = 0f;
            yield return new WaitForSeconds(pauseDuration);

            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }

    //private void FixedUpdate()
    //{
    //    platformVelocity = (rb.position - lastPosition) / Time.fixedDeltaTime;
    //    lastPosition = rb.position;

    //    if (playerRbOnPlatform != null)
    //    {
    //        Vector3 horizontalVel = new Vector3(platformVelocity.x, 0f, platformVelocity.z);
    //        playerRbOnPlatform.velocity += horizontalVel;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                playerRbOnPlatform = otherRb;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb == playerRbOnPlatform)
            {
                playerRbOnPlatform = null;
            }
        }
    }

    public void TransportObject()
    {
        if (objectTransport != null && pos != null)
        {
            objectTransport.transform.position = pos.position;
        }
    }
}