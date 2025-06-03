using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateObject : MonoBehaviour
{
    public Vector3 ejeRotacion = new Vector3(0, 1, 0); // Eje Y por defecto
    public float velocidadRotacion = 45f; // Grados por segundo

    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false; // Asegurate de que la rotación no esté congelada
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(ejeRotacion * velocidadRotacion * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}