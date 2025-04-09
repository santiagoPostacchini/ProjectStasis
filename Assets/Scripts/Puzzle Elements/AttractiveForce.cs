using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractiveForce : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float detectionHeight = 5f;
    [SerializeField] private float pullStrength = 10f;
    [SerializeField] private LayerMask playerLayer;

    private void OnEnable()
    {
        NotifyTrampoline.OnPlayerFalling += TryCatchPlayer;
    }

    private void OnDisable()
    {
        NotifyTrampoline.OnPlayerFalling -= TryCatchPlayer;
    }

    private void TryCatchPlayer(Vector3 playerPos, Rigidbody playerRb)
    {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.up;

        if (Physics.SphereCast(origin, detectionRadius, direction, out RaycastHit hit, detectionHeight, playerLayer))
        {
           
            if (hit.rigidbody == playerRb)
            {
                Vector3 toCenter = (transform.position - playerRb.position);
                float distance = toCenter.magnitude;

                // Fuerza proporcional a la distancia, más cerca => menos fuerza
                Vector3 pullForce = toCenter.normalized * (pullStrength * Mathf.Clamp01(distance / detectionRadius));

                playerRb.AddForce(pullForce, ForceMode.Force);
            }
        }
    }
}
