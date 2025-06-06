using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeableBrokenDoor : MonoBehaviour,IStasis
{
    public bool IsFreezed => _isFreezed;
    private bool _isFreezed;
    private BrokenDoor brokenDoor;
    private Rigidbody objRB;
    //
    public Material matStasis;
    private readonly string _outlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;
    void Start()
    {
        brokenDoor = GetComponent<BrokenDoor>();
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StatisEffectActivate()
    {
        FreezeObject();
    }
    /// <summary>
    /// Deactivates the stasis effect.
    /// </summary>
    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    /// <summary>
    /// Freezes the trampoline by saving its state, stopping motion, and disabling gravity.
    /// </summary>
    private void FreezeObject()
    {
        if (!_isFreezed)
        {
           
            

            _isFreezed = true;
            SetColorOutline(Color.green, 1);
            SetOutlineThickness(1.2f); // Visual cue for stasis.
        }
    }


    /// <summary>
    /// Unfreezes the trampoline by restoring its state and re-enabling gravity.
    /// </summary>
    private void UnfreezeObject()
    {
        if (_isFreezed)
        {
            _isFreezed = false;
            SetOutlineThickness(0f); // Reset visual cue.
            //
        }
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
