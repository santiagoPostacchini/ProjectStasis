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
    public Transform pos; // destino al caer (jugador)

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

    // Punto fijo final hacia donde caer
    private Vector3? fallTarget = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (waypoints.Length < 2)
        {
            Debug.LogError("Necesitás al menos 2 puntos para el movimiento errático.");
            enabled = false;
            return;
        }

        if (lineRenderer == null)
        {
            // Creamos un LineRenderer automáticamente si no hay ninguno
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }

        lineRenderer.enabled = false; // Lo desactivamos al principio

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
        // Actualizamos la línea si estamos en modo caída y tenemos target fijo
        if (isFalling && fallTarget.HasValue)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);    // inicio: posición actual del objeto
            lineRenderer.SetPosition(1, fallTarget.Value);      // fin: posición fija calculada
        }
        else
        {
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

        // Calculamos el punto final de caída solo una vez
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

        // Reseteamos la posición fija para que se calcule una vez en MoveDownErratically
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