using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStaseable : MonoBehaviour
{
    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    [SerializeField] private Renderer _renderer;
    public RotateObjectStasis rotateRoom;

    public List<ChildRoomStaseable> childreenRoom = new List<ChildRoomStaseable>();

    private void Awake()
    {
    }
    private void Start()
    {
        _mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();

    }
    public void ChildreenStasis(bool b)
    {
        if (b)
        {
            foreach (var item in childreenRoom)
            {
                item.StatisEffectActivate();
            }
        }
        else
        {
            foreach (var item in childreenRoom)
            {
                item.StatisEffectDeactivate();
            }
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
