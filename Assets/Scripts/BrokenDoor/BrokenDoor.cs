using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDoor : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform posA;
    public Transform posB;
    public float moveSpeed = 3f;
    public float targetDistance = 0.01f;
    private Vector3 _currentTarget;

    [Header("Colisión entre colliders")]
    public BoxCollider myBoxCollider;
    public BoxCollider otherBoxCollider;

    private Rigidbody rb;
    private FreezeableBrokenDoor freezeScript;

    [Header("Cooldown para cambio de dirección")]
    public float switchCooldown = 1.2f;
    private bool canSwitch = true;
    private float switchTimer = 0f;

    private Vector3 currentPosition; // <- NUEVA VARIABLE

    private void Start()
    {
        _currentTarget = posB.position;
        rb = GetComponent<Rigidbody>();
        freezeScript = GetComponent<FreezeableBrokenDoor>();

        if (myBoxCollider == null)
            myBoxCollider = GetComponent<BoxCollider>();

        currentPosition = rb.position; // Guardar posición inicial
    }

    private void Update()
    {
        HandleCooldown();

        if (freezeScript != null && freezeScript.IsFreezed)
            return;

        if (canSwitch && myBoxCollider != null && otherBoxCollider != null)
        {
            if (myBoxCollider.bounds.Intersects(otherBoxCollider.bounds))
            {
                SwitchTarget();
            }
        }
    }

    private void FixedUpdate()
    {
        if (freezeScript != null && freezeScript.IsFreezed)
        {
            rb.MovePosition(currentPosition); // <- QUEDA FIJO
            return;
        }

        MoveToTargetRigidbody();
    }

    private void MoveToTargetRigidbody()
    {
        Vector3 targetPosition = new Vector3(_currentTarget.x, currentPosition.y, currentPosition.z);

        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        currentPosition = newPosition; // <- ACTUALIZA POSICIÓN SOLO CUANDO NO ESTÁ CONGELADO

        if (Mathf.Abs(newPosition.x - targetPosition.x) < targetDistance)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        _currentTarget = (_currentTarget == posA.position) ? posB.position : posA.position;
        canSwitch = false;
        switchTimer = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canSwitch && collision.collider == otherBoxCollider)
        {
            SwitchTarget();
        }
    }

    private void HandleCooldown()
    {
        if (!canSwitch)
        {
            switchTimer += Time.deltaTime;
            if (switchTimer >= switchCooldown)
            {
                canSwitch = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (posA != null && posB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(posA.position, posB.position);
        }

        if (myBoxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(myBoxCollider.bounds.center, myBoxCollider.bounds.size);
        }

        if (otherBoxCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(otherBoxCollider.bounds.center, otherBoxCollider.bounds.size);
        }
    }
}