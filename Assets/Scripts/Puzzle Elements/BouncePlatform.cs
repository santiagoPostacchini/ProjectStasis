using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [Range(0f, 40f)]
    [SerializeField]private float forceImpulse;
    private bool already = false;
    private void OnTriggerEnter(Collider other)
    {
        if (already) return;
        AddForceToPlayer(other);
    }
    public void AddForceToPlayer(Collider other)
    {
        Debug.Log("Colisiona con la plataforma");
        Rigidbody rb = other.gameObject.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir = transform.up;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * forceImpulse, ForceMode.Impulse); // Ajusta la fuerza
        }
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        already = true;
        yield return new WaitForSeconds(0.5f);
        already = false;
    }
}
