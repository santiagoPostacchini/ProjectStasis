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
    public Transform pos;

    [Header("Paredes")]
    public List<GameObject> paredes;

    [Header("Visual Settings")]
    public float fallOffsetRadius = 3f;

    [Header("Line Renderer (asignar en inspector o se crea automáticamente)")]
    public LineRenderer lineRenderer;

    public bool isFreezed;
    private bool isFalling = false;

    private int currentTargetIndex;
    private float timer;

    public Rigidbody rb;

    private Vector3 lastDirection;
    private Vector3? fallTarget = null;

    private Vector3 currentRandomOffset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waypoints.Length < 2)
        {
            Debug.LogError("Necesitás al menos 2 puntos para el movimiento errático.");
            enabled = false;
            return;
        }

        if (lineRenderer != null) lineRenderer.enabled = false;

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

    private void Update()
    {
        if (isFalling && fallTarget.HasValue)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, fallTarget.Value);
        }
        else
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
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
        if (pos == null) return;

        if (fallTarget == null)
        {
            Vector2 circleOffset = Random.insideUnitCircle.normalized * fallOffsetRadius;
            Vector3 lateralOffset = new Vector3(circleOffset.x, 0f, circleOffset.y);
            fallTarget = pos.position + lateralOffset;
        }

        Vector3 directionToTarget = (fallTarget.Value - rb.position).normalized;
        lastDirection = directionToTarget;

        Vector3 movement = directionToTarget * speed * fallSpeedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void ApplyContinuousRotation()
    {
        Quaternion deltaRotation = Quaternion.Euler(rotationAxis * rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private Vector3 GetTargetWithRandomness()
    {
        return waypoints[currentTargetIndex].position + currentRandomOffset;
    }

    private void ChooseNewTarget()
    {
        int newIndex;
        do
        {
            newIndex = Random.Range(0, waypoints.Length);
        } while (newIndex == currentTargetIndex);

        currentTargetIndex = newIndex;

        currentRandomOffset = new Vector3(
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness),
            Random.Range(-randomness, randomness)
        );
    }

    public void EnterFallMode()
    {
        isFalling = true;
        timer = 0f;
        fallTarget = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (paredes.Contains(collision.gameObject))
        {
            Vector3 reversed = -lastDirection.normalized;

            if (isFalling)
            {
                lastDirection = reversed;
            }
            else
            {
                Vector3 newTarget = rb.position + reversed * speed;
                int closestIndex = GetClosestWaypointIndex(newTarget);
                currentTargetIndex = closestIndex;

                currentRandomOffset = new Vector3(
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness),
                    Random.Range(-randomness, randomness)
                );
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