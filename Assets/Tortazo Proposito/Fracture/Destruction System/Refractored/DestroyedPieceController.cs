using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedPieceController : MonoBehaviour,IStasis
{
    public bool is_connected = true;
    [HideInInspector] public bool visited = false;
    public List<DestroyedPieceController> connected_to;

    public static bool is_dirty = false;

    [SerializeField]private Rigidbody _rigidbody;
    private Vector3 _starting_pos;
    private Quaternion _starting_orientation;
    private Vector3 _starting_scale;

    private bool _configured = false;
    private bool _connections_found = false;

    public bool IsFreezed => isFreezed;
    public bool isFreezed;
    public bool wasHit = false;


    public Material matStasis;
    public readonly string _outlineThicknessName = "_BorderThickness";
    public MaterialPropertyBlock _mpb;
    public Renderer _renderer;
    public MeshCollider meshCollider;
    public MeshCollider meshColliderTrigger;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        connected_to = new List<DestroyedPieceController>();
        _starting_pos = transform.position;
        _starting_orientation = transform.rotation;
        _starting_scale = transform.localScale;

        transform.localScale *= 1.02f;

        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
       
        if (!_configured)
        {
            var neighbour = collision.gameObject.GetComponent<DestroyedPieceController>();
            if (neighbour)
            {
                if(!connected_to.Contains(neighbour))
                    connected_to.Add(neighbour);
            }
        }
        //else if (collision.gameObject.CompareTag("Floor"))
        //{
        //    VFXController.Instance.spawn_dust_cloud(transform.position);
        //}
    }
    private void Update()
    {
        if (wasHit)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
    public void make_static()
    {
        _configured = true;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = true;

        transform.localScale = _starting_scale;
        transform.position = _starting_pos;
        transform.rotation = _starting_orientation;
    }

    public void cause_damage(Vector3 force)
    {
        is_connected = false;
        _rigidbody.isKinematic = false;
        is_dirty = true;
        _rigidbody.AddForce(force, ForceMode.Impulse);
        VFXController.Instance.spawn_dust_cloud(transform.position);
        
    }

    public void drop()
    {
        is_connected = false;
        _rigidbody.isKinematic = false;
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
        if (!isFreezed)
        {
            if (is_connected) return;
            isFreezed = true;
            SetOutlineThickness(1.05f);
            SetColorOutline(Color.green, 1f);
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            // Congelar posición y rotación para prevenir movimientos
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }



    private void UnfreezeObject()
    {
        if (!isFreezed) return;
        isFreezed = false;
        SetOutlineThickness(0f);
        Color lightRed = new Color(135, 93, 93);
        SetColorOutline(Color.white, 1f);
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        // Congelar posición y rotación para prevenir movimientos
        _rigidbody.constraints = RigidbodyConstraints.None;

    }


    public void SetOutlineThickness(float thickness)
    {
        if (_renderer == null || _mpb == null) return;
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
