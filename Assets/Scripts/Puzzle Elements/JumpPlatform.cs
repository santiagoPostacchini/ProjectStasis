using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f; 
    [SerializeField] private float forwardForce = 5f; 

    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            Vector3 jumpDirection = (transform.forward * forwardForce) + (transform.up * jumpForce);
            rb.velocity = Vector3.zero; // Reinicia la velocidad para evitar fuerzas acumuladas
            rb.AddForce(jumpDirection, ForceMode.VelocityChange);
        }
    }
}
