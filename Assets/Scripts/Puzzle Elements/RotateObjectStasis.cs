using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateObjectStasis : MonoBehaviour,IStasis
{
    public Vector3 ejeRotacion = new Vector3(0, 1, 0); // Eje Y por defecto
    public float velocidadRotacion = 45f; // Grados por segundo

    public Rigidbody rb;
    private bool _isFreezed = false;
    public bool IsFreezed => _isFreezed;


    public Material matStasis;
    private readonly string _outlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;

    void Start()
    {

        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false; // Asegurate de que la rotación no esté congelada
    }

    void FixedUpdate()
    {
        if (!_isFreezed)
        {
            Quaternion deltaRotation = Quaternion.Euler(ejeRotacion * velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        
    }

    public void StatisEffectActivate()
    {
        FreezeObject();
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }
    private void FreezeObject()
    {
        if (!_isFreezed)
        {
            _isFreezed = true;

            SetOutlineThickness(1.05f);
            SetColorOutline(Color.green, 1f);
        }
    }


   
    private void UnfreezeObject()
    {
        if (!_isFreezed) return;
        _isFreezed = false;
        SetOutlineThickness(0f);
        Color lightGreen = new Color(0.6f, 1f, 0.6f);
        SetColorOutline(lightGreen, 1f);

    }


    public void SetOutlineThickness(float thickness)
    {
        if (!_renderer || _mpb == null) return;
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_outlineThicknessName, thickness);
        _renderer.SetPropertyBlock(_mpb);
    }

    public void SetColorOutline(Color color, float alpha)
    {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(_mpb);
    }
}