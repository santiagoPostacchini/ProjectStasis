using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 ejeRotacion = new Vector3(0, 1, 0); // Eje Y por defecto
    public float velocidadRotacion = 45f; // Grados por segundo

    public Rigidbody rb;

    public BladeStasis[] blades;
    public bool canRotate;


    public Material matStasis;
    public readonly string _outlineThicknessName = "_BorderThickness";
    public MaterialPropertyBlock _mpb;
    public Renderer _renderer;
    private void Start()
    {

        rb = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    void FixedUpdate()
    {
        if (canRotate)
        {
            Quaternion deltaRotation = Quaternion.Euler(ejeRotacion * velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }

    }
}
