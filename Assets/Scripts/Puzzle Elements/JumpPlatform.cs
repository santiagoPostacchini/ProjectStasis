using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f; 
    [SerializeField] private float forwardForce = 5f;
    [SerializeField] private Transform pos;
    private float duration = 0.05f;
    private bool canApplyForce = true;
    private void OnCollisionEnter(Collision collision)
    {
       
        if (!canApplyForce) return;
        Rigidbody rb = collision.rigidbody;
        Transform t = collision.transform;
        if (rb != null)
        {
            StartCoroutine(ApplyJumpForce(rb, t));
        }
    }
    private void Update()
    {
        
    }
    private IEnumerator ApplyJumpForce(Rigidbody rb, Transform t)
    {
        canApplyForce = false;

        Vector3 startPos = t.position;
        Vector3 endPos = pos.position;
   

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        float time = 0f;
        while (time < duration)
        {
            float tLerp = time / duration;
            t.position = Vector3.Lerp(startPos, endPos, tLerp);
            time += Time.deltaTime;
            yield return null;
        }

        // Aseguramos posición exacta
        t.position = endPos;

        // Esperamos un frame antes de volver a activar la física
        yield return null;
        rb.isKinematic = false;

        // APLICAMOS LA FUERZA JUSTO DESPUÉS DE ACTIVAR LA FÍSICA
        yield return new WaitForFixedUpdate(); // Esperamos al siguiente FixedUpdate para que la física esté lista
        yield return null;

        Vector3 jumpDirection = (transform.forward * forwardForce) + (transform.up * jumpForce);
        rb.AddForce(jumpDirection, ForceMode.VelocityChange);

        // Cooldown para evitar múltiples disparos seguidos
        yield return new WaitForSeconds(0.5f);
        canApplyForce = true;
    }
}
