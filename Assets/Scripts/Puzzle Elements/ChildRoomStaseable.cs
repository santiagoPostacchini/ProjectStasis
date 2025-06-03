using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRoomStaseable : MonoBehaviour,IStasis
{
    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    [SerializeField] private Renderer _renderer;
    

    public RoomStaseable room;
    public bool isFreezed;
    private void Awake()
    {
        // Intentamos obtener el FallingRoof del mismo objeto si no está asignado
        room = GetComponentInParent<RoomStaseable>();
    }
    private void Start()
    {
        _mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        room.childreenRoom.Add(this);
        _renderer = GetComponent<MeshRenderer>();
    }


    public void StatisEffectActivate()
    {
        FreezeObject();
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }
    public void NotifyRoom(bool b)
    {
        room.ChildreenStasis(b);
    }
    private void FreezeObject()
    {
        if (!isFreezed)
        {

            room.rotateRoom.rb.velocity = Vector3.zero;
            room.rotateRoom.rb.angularVelocity = Vector3.zero;
            room.rotateRoom.rb.useGravity = false;
            isFreezed = true;

            SetColorOutline(Color.green, 1);
            SetOutlineThickness(1.05f);
            NotifyRoom(true);
        }
    }

    private void UnfreezeObject()
    {
        if (isFreezed)
        {

            isFreezed = false;
            room.rotateRoom.rb.useGravity = false;

            SetOutlineThickness(1f);
            NotifyRoom(false);
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
