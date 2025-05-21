using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionDetector))]
public abstract class PhysicsObject : MonoBehaviour
{
    public Player.Player player;
    // Common physics components
    public Rigidbody objRB;
    protected CollisionDetector collisionDetector;
    protected Transform objGrabPointTransform;

    public bool _isFreezed;
    // For outline visual effects
    public Material matStasis;
    private string OutlineThicknessName = "_BorderThickness";
    private MaterialPropertyBlock _mpb;
    [SerializeField]private Renderer _renderer;

    // Configuration for collision impulse handling
    public float collisionCheckDelay = 0.3f;
    [HideInInspector] public bool IsUnderHighPressure;
    [SerializeField] private float _rigidbodyPressureThreshold = 50f;
    [SerializeField] private float _nonRigidbodyPressureThreshold = 30f;

    // Variables for accumulating collision impulse
    private float accumulatedImpulse = 0f;
    private float accumulationTime = 0f;
    private float accumulationPeriod = 0.1f;
    protected bool _canCheckCollisions = false;

    // Variables for saving Rigidbody state (used by stasis objects)
    protected Vector3 _savedVelocity;
    protected Vector3 _savedAngularVelocity;
    protected bool _savedKinematic;
    protected float _savedDrag;

    protected virtual void Start()
    {
        objRB = GetComponent<Rigidbody>();
        collisionDetector = GetComponent<CollisionDetector>();

        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        SetOutlineThickness(1f);

        StartCoroutine(EnableCollisionCheckAfterDelay(collisionCheckDelay));
    }

    // Grabs the object and sets up physics for a smooth movement.
    public virtual void Grab(Transform grabPoint)
    {
        objGrabPointTransform = grabPoint;
        objRB.useGravity = false;
        objRB.drag = 10;
    }

    // Drops the object, reverting physics settings.
    public virtual void Drop()
    {
        objGrabPointTransform = null;
        objRB.useGravity = true;
        objRB.drag = 1;
    }

    // Throws the object in a given direction.
    public virtual void Throw(Transform grabPoint, float force)
    {
        Drop();
        Vector3 throwVelocity = grabPoint.forward * (force / objRB.mass);
        objRB.AddForce(throwVelocity);
    }

    // Handles movement of the object when grabbed.
    protected virtual void FixedUpdate()
    {
        if (objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.rotation, objGrabPointTransform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MovePosition(newPos);
            objRB.MoveRotation(newRot);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_canCheckCollisions)
            return;

        float currentImpulseForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        accumulatedImpulse += currentImpulseForce;
        accumulationTime += Time.fixedDeltaTime;

        if (accumulationTime >= accumulationPeriod)
        {
            float averageImpulse = accumulatedImpulse / accumulationTime;
            float threshold = collisionDetector.IsHittingNOTRB ? _nonRigidbodyPressureThreshold : _rigidbodyPressureThreshold;
            if (averageImpulse > threshold)
            {
                // Optionally, you may wish to check if the object is currently grabbed.
                if (IsGrabbed)
                {
                    IsUnderHighPressure = true;
                    Drop();
                    StartCoroutine(ResetForcesAfterDrop());
                }
            }
            else
            {
                IsUnderHighPressure = false;
            }

            accumulatedImpulse = 0f;
            accumulationTime = 0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsUnderHighPressure = false;
        accumulatedImpulse = 0f;
        accumulationTime = 0f;
    }

    protected IEnumerator ResetForcesAfterDrop()
    {
        yield return new WaitForFixedUpdate();
        if (objRB != null)
        {
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    protected IEnumerator EnableCollisionCheckAfterDelay(float delay)
    {
        _canCheckCollisions = false;
        yield return new WaitForSeconds(delay);
        _canCheckCollisions = true;
    }

    // Saves the current Rigidbody state.
    public void SaveRigidbodyState()
    {
        if (objRB != null)
        {
             _savedKinematic = GetComponent<Rigidbody>().isKinematic;
            _savedVelocity = objRB.velocity;
            _savedAngularVelocity = objRB.angularVelocity;
            _savedDrag = objRB.drag;
        }
    }

    // Restores the Rigidbody state saved earlier.
    public void RestoreRigidbodyState()
    {
        if (objRB != null)
        {
            
            objRB.velocity = _savedVelocity;
            objRB.angularVelocity = _savedAngularVelocity;
            objRB.drag = _savedDrag;
            objRB.isKinematic = _savedKinematic;
            objRB.WakeUp();
        }
    }
    
    // Updates the outline effect (visual feedback) on the object.
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

    // Indicates whether the object is currently grabbed.
    public bool IsGrabbed
    {
        get { return objGrabPointTransform != null; }
    }
    public void Glow(bool enable, float intensity = 1f)
    {
        if (_renderer != null && _renderer.materials.Length > 0)
        {
            Material[] mats = _renderer.materials;
            Material baseMat = mats[0];

            if (enable)
            {
                if (_isFreezed) return;
                // Habilitar el efecto emissive
                baseMat.EnableKeyword("_EMISSION");

                // Verificar si ya tiene color de emisi�n
                Color currentEmission = baseMat.GetColor("_EmissionColor");

                // Si no tiene color de emisi�n, empezar con un valor bajo como el gris
                if (currentEmission == Color.black)
                    currentEmission = new Color(1f,0f,0f); 

                // Aplicar la intensidad al color de emisi�n (multiplicamos por la intensidad)
                baseMat.SetColor("_EmissionColor", currentEmission * intensity);
            }
            else
            {
                // Deshabilitar el efecto emissive
                baseMat.DisableKeyword("_EMISSION");

                // Apagar la emisi�n (ponemos el color a negro)
                baseMat.SetColor("_EmissionColor", Color.black);
            }

            // Actualizar los materiales del renderer
            _renderer.materials = mats;
        }
    }

}