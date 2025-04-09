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

    private Rigidbody rb;
    private Vector3 target;

    private void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        target = pointB.position;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        Vector3 newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector3.Distance(rb.position, target) < 0.1f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }
}
