using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeStasis : MonoBehaviour,IStasis
{
    
    public bool _isFreezed = false;
    public bool IsFreezed => _isFreezed;

    public BladeStasis[] otherBlades;
    public RotateObject father;

    
    // Start is called before the first frame update
    void Start()
    {

        father.rb.freezeRotation = false; // Asegurate de que la rotación no esté congelada
    }

    public void FreezeOtherBlades()
    {
        foreach (var item in otherBlades)
        {
            item._isFreezed = IsFreezed;
            item.FreezeObject();
        }
    }
    public void UnFreezeOtherBlades()
    {
        foreach (var item in otherBlades)
        {
            item._isFreezed = IsFreezed;
            item.UnfreezeObject();
        }
    }
    public bool AllBladesUnFreezed()
    {
        foreach (var item in otherBlades)
        {
            if (item.IsFreezed == true) return false;
        }
        if (IsFreezed == true) return false;
        return true;
    }

    public void StatisEffectActivate()
    {
        FreezeObject();
        FreezeOtherBlades();
        father.canRotate = AllBladesUnFreezed();
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
        UnFreezeOtherBlades();
        father.canRotate = AllBladesUnFreezed();
    }
    private void FreezeObject()
    {
        if (!_isFreezed)
        {
            _isFreezed = true;
            father.rb.isKinematic = true;
            SetOutlineThickness(1.05f);
            SetColorOutline(Color.green, 1f);
        }
    }


   
    private void UnfreezeObject()
    {
        if (!_isFreezed) return;
        _isFreezed = false;
        father.rb.isKinematic = false;
        SetOutlineThickness(0f);
        Color lightGreen = new Color(0.6f, 1f, 0.6f);
        SetColorOutline(lightGreen, 1f);

    }


    public void SetOutlineThickness(float thickness)
    {
        if (!father._renderer || father._mpb == null) return;
        father._renderer.GetPropertyBlock(father._mpb);
        father._mpb.SetFloat(father._outlineThicknessName, thickness);
        father._renderer.SetPropertyBlock(father._mpb);
    }

    public void SetColorOutline(Color color, float alpha)
    {
        father._renderer.GetPropertyBlock(father._mpb);

        father._mpb.SetColor("_Color", color);
        father._renderer.SetPropertyBlock(father._mpb);
    }
}