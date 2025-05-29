using System.Collections;
using UnityEngine;

public class FallingRoof : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float pauseBeforeFall = 1f;

    private Rigidbody rb;
    private Vector3 target;
    private bool hasStartedMoving = false;
    public bool isFreezed = false; // Puedes controlar este flag desde afuera para congelar

    public float vibrationMagnitude;
    public float vibrationDuration;
    public CameraShake cameraShake;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        transform.position = pointA.position;  // Aseguramos que empieza en A
        target = pointB.position;
    }
    public void CameraShake()
    {
        cameraShake.Shake(vibrationDuration, vibrationMagnitude);
    }
    public void StartFalling()
    {
        if (hasStartedMoving) return;

        hasStartedMoving = true;
        StartCoroutine(MoveToTarget());
        CameraShake();
    }

    private bool DistanceToPointBLess1()
    {
        return Vector3.Distance(transform.position, pointB.transform.position) < 1f;
    }

    private IEnumerator MoveToTarget()
    {
        
        yield return new WaitForSeconds(pauseBeforeFall);

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            if (isFreezed)
            {
                yield return null; // espera 1 frame si está congelado y chequea de nuevo
                continue;
            }

            Vector3 direction = (target - transform.position).normalized;
            Vector3 nextPos = transform.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
            if (DistanceToPointBLess1())
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);
        rb.isKinematic = false; // Por ejemplo, para que caiga con gravedad después de llegar
    }
}