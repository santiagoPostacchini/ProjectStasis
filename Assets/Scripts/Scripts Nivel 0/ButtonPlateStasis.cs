using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle_Elements;

public class ButtonPlateStasis : MonoBehaviour,IStasis
{

    public PressurePlate _pressurePlate;

    public Material matStasis;
    private readonly string _outlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;

    public bool IsFreezed => _pressurePlate.isFrozen;

    private void Start()
    {
        _pressurePlate.isFrozen = false;
        _pressurePlate = GetComponent<PressurePlate>();
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        SetOutlineThickness(0f);
    }
    public void StatisEffectActivate()
    {
        FreezeObject();
        Debug.Log("AAAA");
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    public void FreezeObject()
    {
        if (!_pressurePlate.isFrozen)
        {
            _pressurePlate.isFrozen = true;

            SetOutlineThickness(1.2f);
            SetColorOutline(Color.green, 1f);
        }
    }


    public void UnfreezeObject()
    {
        if (!_pressurePlate) return;
        _pressurePlate.isFrozen = false;
        SetOutlineThickness(0f);
        SetColorOutline(Color.white, 0);

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
