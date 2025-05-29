using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class ErraticObject : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    public float speed = 3f;
    public float randomness = 0.5f;
    public float switchTime = 2f;

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);
    public float rotationSpeed = 90f;

    [Header("Frenetic Fall Settings")]
    public float fallSpeedMultiplier = 2.5f;
    public Transform pos; // destino al caer

    [Header("Paredes")]
    public List<GameObject> paredes;

    public bool isFreezed;
    private bool isFalling = false;

    private int currentTargetIndex;
    private float timer;

    public Rigidbody rb;

    private Vector3 lastDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waypoints.Length < 2)
        {
            Debug.LogError("Necesitás al menos 2 puntos para el movimiento errático.");
            enabled = false;
            return;
        }

        ChooseNewTarget();
    }

    private void FixedUpdate()
    {
        if (isFreezed) return;

        timer += Time.fixedDeltaTime;

        if (isFalling)
        {
            MoveDownErratically();
        }
        else
        {
            MoveTowardsCurrentTarget();

            if (timer >= switchTime)
            {
                ChooseNewTarget();
                timer = 0f;
            }
        }

        ApplyContinuousRotation();
    }

    private void MoveTowardsCurrentTarget()
    {
        Vector3 targetPosition = GetTargetWithRandomness();
        Vector3 moveDirection = (targetPosition - rb.position).normalized;
        lastDirection = moveDirection;

        Vector3 movement = moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void MoveDownErratically()
    {
        Vector3 descent = Vector3.down;
        Vector3 directionToPos = Vector3.zero;

        if (pos != null)
        {
            directionToPos = (pos.position - rb.position).normalized * 4f;
        }

        Vector3 dynamicFallDirection = (directionToPos + descent).normalized;
        lastDirection = dynamicFallDirection;

        Vector3 movement = dynamicFallDirection * speed * fallSpeedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void ApplyContinuousRotation()
    {
        Quaternion deltaRotation = Quaternion.Euler(rotationAxis * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private Vector3 GetTargetWithRandomness()
    {
        Vector3 targetPos = waypoints[currentTargetIndex].position;
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness)
        );
        return targetPos + randomOffset;
    }

    private void ChooseNewTarget()
    {
        int newIndex;
        do
        {
            newIndex = Random.Range(0, waypoints.Length);
        } while (newIndex == currentTargetIndex);

        currentTargetIndex = newIndex;
    }

    public void EnterFallMode()
    {
        isFalling = true;
        timer = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (paredes.Contains(collision.gameObject))
        {
            Vector3 reversed = -lastDirection.normalized;

            if (isFalling)
            {
                // Cambia la dirección de caída temporalmente
                // (Se recalculará igual cada frame, pero mejora el rebote inmediato)
                lastDirection = reversed;
            }
            else
            {
                // Retrocede y busca el waypoint más cercano a esa nueva dirección
                Vector3 newTarget = rb.position + reversed * speed;
                int closestIndex = GetClosestWaypointIndex(newTarget);
                currentTargetIndex = closestIndex;
            }

            timer = 0f;
        }
    }

    private int GetClosestWaypointIndex(Vector3 position)
    {
        float closestDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(position, waypoints[i].position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}