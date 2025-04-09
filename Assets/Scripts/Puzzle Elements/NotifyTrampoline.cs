using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotifyTrampoline : MonoBehaviour
{
    public static event Action<Vector3, Rigidbody> OnPlayerFalling;

    private Rigidbody rb;
  

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
    }
    private void FixedUpdate()
    {
        if (rb.velocity.y < -0.5f) // Está cayendo
        {
            OnPlayerFalling?.Invoke(transform.position, rb);
        }
    }
}
