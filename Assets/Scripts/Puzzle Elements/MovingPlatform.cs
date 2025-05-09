using System.Collections;
using System.Collections.Generic;
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
    private void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody>();
        target = pointB.position;
    }
    private void Update()
    {
        TransportObject();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(rb.position, target) < 0.05f)
        {
            // Cambia suavemente al nuevo objetivo cuando está muy cerca
            target = target == pointA.position ? pointB.position : pointA.position;
        }

        // Movimiento suave hacia el objetivo
        Vector3 newPosition = Vector3.SmoothDamp(rb.position, target, ref velocity, 0.1f, speed, Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.parent = null;
        }
    }
    public void TransportObject()
    {
        if(objectTransport != null && pos != null)
        {
            objectTransport.transform.position = pos.transform.position;
        }
    }
}
