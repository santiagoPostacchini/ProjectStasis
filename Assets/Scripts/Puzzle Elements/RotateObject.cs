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
    private bool AlreadyNotify = false;
    
    public Material matStasis;
    public readonly string _outlineThicknessName = "_BorderThickness";
    public MaterialPropertyBlock _mpb;
    public Renderer _renderer;

    public GameObject triggerObject;

    private FanColisionDetector _fanColisionDetector;

    private void Start()
    {
        _fanColisionDetector = GetComponent<FanColisionDetector>();
        rb = GetComponent<Rigidbody>();
        _mpb = new MaterialPropertyBlock();
    }

    void FixedUpdate()
    {
        if (canRotate)
        {
            Quaternion deltaRotation = Quaternion.Euler(ejeRotacion * velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        else
        {
            if (!AlreadyNotify)
            {
                _fanColisionDetector.ResetTimerScaler();
                triggerObject.SetActive(false);
                AlreadyNotify = true;
            }
        }

    }
}
