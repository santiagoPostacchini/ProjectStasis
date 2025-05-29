using System.Collections;
using UnityEngine;

public class FreezableFallingRoof : MonoBehaviour, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;


    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    [SerializeField] private Renderer _renderer;

    [SerializeField] private FallingRoof fallingRoof;

    

    private void Awake()
    {
        // Intentamos obtener el FallingRoof del mismo objeto si no está asignado
        if (fallingRoof == null)
            fallingRoof = GetComponent<FallingRoof>();
    }
    private void Start()
    {
        _mpb = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();

    }


    public void StatisEffectActivate()
    {
        FreezeObject();
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
    }

    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    private void FreezeObject()
    {
        if (!fallingRoof.isFreezed)
        {


            fallingRoof.isFreezed = true;

            SetColorOutline(Color.green, 1);
            SetOutlineThickness(1.05f);
        }
    }

    private void UnfreezeObject()
    {
        if (fallingRoof.isFreezed)
        {

            fallingRoof.isFreezed = false;

            

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