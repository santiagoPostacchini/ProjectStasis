using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezableErraticObject : MonoBehaviour,IStasis
{

    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    [SerializeField] private Renderer _renderer;

    [SerializeField] private ErraticObject erraticObject;

    public bool IsFreezed => erraticObject.isFreezed;

    private void Awake()
    {
        // Intentamos obtener el FallingRoof del mismo objeto si no está asignado
        if (erraticObject == null)
            erraticObject = GetComponent<ErraticObject>();
    }
    private void Start()
    {
        _mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();

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
        if (!erraticObject.isFreezed)
        {

            erraticObject.rb.velocity = Vector3.zero;
            erraticObject.rb.angularVelocity = Vector3.zero;
            erraticObject.rb.useGravity = false;
            erraticObject.rb.isKinematic = true;
            erraticObject.isFreezed = true;

            SetColorOutline(Color.green, 1);
            SetOutlineThickness(1.05f);
        }
    }

    private void UnfreezeObject()
    {
        if (erraticObject.isFreezed)
        {

            erraticObject.isFreezed = false;



            SetOutlineThickness(1f);
        }
    }
    public void SetOutlineThickness(float thickness)
    {
        if (_renderer != null && _mpb != null)
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(OutlineThicknessName, thickness);
            // _mpb.SetColor("_Color", Color.green);
            _renderer.SetPropertyBlock(_mpb);
            //Glow(false, 1);
        }
    }
    public void SetColorOutline(Color color, float alpha)
    {
        _renderer.GetPropertyBlock(_mpb);
        //_mpb.SetFloat("_Alpha", alpha);

        _mpb.SetColor("_Color", color);
        _renderer.SetPropertyBlock(_mpb);
    }

}
