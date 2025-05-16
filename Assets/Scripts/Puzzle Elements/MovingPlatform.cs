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

    public bool canMove;
    public GameObject objectTransport;
    public Transform pos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        target = pointB.position;
        canMove = true;
    }
    private void Update()
    {
        TransportObject();
    }
    private void FixedUpdate()
    {
        if (!canMove) return;

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
    }

    private IEnumerator SwapTargetAfterPause()
    {
        canMove = false;
        yield return new WaitForSeconds(pauseDuration);
        target = target == pointA.position ? pointB.position : pointA.position;
        canMove = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // Solo seteamos si no tiene padre o no está ya en esta plataforma
            if (player.transform.parent != transform)
            {
                player.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && player.transform.parent == transform)
        {
            player.transform.SetParent(null);
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