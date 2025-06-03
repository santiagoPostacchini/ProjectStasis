using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float pauseDuration = 0.5f;

    private Rigidbody rb;
    private Vector3 target;
    private Vector3 previousPosition;

    public bool canMove;
    public GameObject objectTransport;
    public Transform pos;

    [SerializeField]private Transform passenger; // Referencia al jugador encima

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (pointB != null) target = pointB.position;
        canMove = true;

        previousPosition = transform.position;
    }

    private void Update()
    {
        TransportObject();
    }

    private void FixedUpdate()
    {
        if (!canMove || pointA == null || pointB == null)
        {
            rb.velocity = Vector3.zero;
            return;
        }
            

        Vector3 direction = (target - rb.position).normalized;
        float distance = Vector3.Distance(rb.position, target);

        if (distance > 0.05f)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(SwapTargetAfterPause());
        }

        MovePassenger(); // Mover jugador encima si lo hay
        previousPosition = transform.position;
    }

    private IEnumerator SwapTargetAfterPause()
    {
        canMove = false;
        yield return new WaitForSeconds(pauseDuration);
        target = target == pointA.position ? pointB.position : pointA.position;
        canMove = true;
    }

    private void MovePassenger()
    {
        if (passenger != null)
        {
            Vector3 delta = transform.position - previousPosition;
            passenger.Translate(delta, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            passenger = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && passenger == other.transform)
        {
            passenger = null;
        }
    }

    public void TransportObject()
    {
        if (objectTransport != null && pos != null)
        {
            objectTransport.transform.position = pos.transform.position;
        }
    }
}