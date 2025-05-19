using UnityEngine;
using System.Collections;

public class PhysicsBox : PhysicsObject, IStasis
{
    private Vector3 freezePosition;
    private Quaternion freezeRotation;
    


    [SerializeField]private LineRenderer _lineRenderer;
    [SerializeField]private int _pointsCount = 50;
    [SerializeField] private float _timeStep = 0.1f;
    [SerializeField] LayerMask _collisionMask;

    //[Header("Soporte de flotación")]
    //[Tooltip("Transform que marca la posición en la que debe ‘flotar’ el cubo al agarrarlo")]
    //[SerializeField] private Transform cubeSupport;
    //[Tooltip("Offset local desde el soporte")]
    //[SerializeField] private Vector3 supportOffset = Vector3.zero;

    //[Header("Partículas de energía")]
    //[Tooltip("ParticleSystem que genera el campo de energía")]
    //[SerializeField] private ParticleSystem energyFieldParticles;

    //private void Awake()
    //{
    //    if (cubeSupport != null)
    //    {
    //        // Posición global + offset local
    //        Vector3 initialPos = cubeSupport.position + cubeSupport.rotation * supportOffset;
    //        transform.position = initialPos;
    //        transform.rotation = cubeSupport.rotation;
    //        // Asegura también la posición del Rigidbody
    //        objRB.MovePosition(initialPos);
    //        objRB.MoveRotation(cubeSupport.rotation);
    //    }
    //}


    public bool IsFreezed
    {
        get => _isFreezed;
        private set => _isFreezed = value;
    }
    public override void Grab(Transform grabPoint)
    {
        base.Grab(grabPoint);
        _lineRenderer.positionCount = 0;

        //// Detenemos el campo de partículas
        //if (energyFieldParticles != null && energyFieldParticles.isPlaying)
        //{
        //    energyFieldParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        //}
        // Additional stasis-related logic on grab can be added here if needed.
    }

    public override void Drop()
    {
        base.Drop();
        if (_isFreezed)
        {
            // Store the current transform values if the object is frozen.
            freezePosition = transform.position;
            freezeRotation = transform.rotation;
            if (_savedVelocity.magnitude > 0.5f)
            {
                DrawTrajectory(transform.position, _savedVelocity, objRB.drag);
            }
        }
    }

    public override void Throw(Transform grabPoint, float force)
    {
        Drop();
        if (!_isFreezed)
        {
            Vector3 throwVelocity = grabPoint.forward * (force / objRB.mass);
            objRB.AddForce(throwVelocity);
        }
    }

    protected override void FixedUpdate()
    {
        // 1) Si está siendo agarrado, usar el grabPoint (Lerp hacia él)
        if (objGrabPointTransform != null)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, objGrabPointTransform.position, Time.fixedDeltaTime * 10f);
            Quaternion newRot = Quaternion.Lerp(transform.rotation, objGrabPointTransform.rotation, Time.fixedDeltaTime * 10f);
            objRB.MovePosition(newPos);
            objRB.MoveRotation(newRot);
        }
        //// 2) Si no está agarrado y no está en stasis, volver al soporte
        //else if (!_isFreezed && cubeSupport != null)
        //{
        //    Vector3 targetPos = cubeSupport.position + cubeSupport.rotation * supportOffset;
        //    objRB.MovePosition(targetPos);
        //    objRB.MoveRotation(cubeSupport.rotation);
        //}
        // 3) Si está en stasis, mantener posición/rotación congeladas
        else if (_isFreezed)
        {
            objRB.MovePosition(freezePosition);
            objRB.MoveRotation(freezeRotation);
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
        }
    }

    // Activates the stasis effect and freezes the object.
    public void StatisEffectActivate()
    {
        FreezeObject();
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
    }

    // Deactivates the stasis effect and unfreezes the object.
    public void StatisEffectDeactivate()
    {
        UnfreezeObject();
    }

    private void FreezeObject()
    {
        if (!_isFreezed)
        {
            SaveRigidbodyState();
            objRB.velocity = Vector3.zero;
            objRB.angularVelocity = Vector3.zero;
            objRB.useGravity = false;
            _isFreezed = true;
            if(_savedVelocity.magnitude > 0.5f)
            {
                DrawTrajectory(transform.position, _savedVelocity, objRB.drag);
            }
            SetColorOutline(Color.green, 1f);
            SetOutlineThickness(1.05f); // Optionally update the visual to indicate stasis.
        }
    }

    public void UnfreezeObject()
    {
        if (_isFreezed)
        {
            RestoreRigidbodyState();
            _isFreezed = false;
            if(objRB)
                objRB.useGravity = true;
            SetOutlineThickness(1f); // Reset the visual effect.
            if(_lineRenderer != null)
            {
                _lineRenderer.positionCount = 0;
            }
            
        }
    }
    public void DrawTrajectory(Vector3 startPos, Vector3 startVelocity, float drag)
    {
        Vector3[] points = new Vector3[_pointsCount];
        Vector3 currentPosition = startPos;
        Vector3 velocity = startVelocity;

        Vector3 gravity = Physics.gravity;
        points[0] = currentPosition;
        int i = 1;

        for (; i < _pointsCount; i++)
        {
            // Aplicar drag: reducción exponencial por unidad de tiempo
            velocity *= 1f / (1f + drag * _timeStep);

            // Aplicar gravedad
            Vector3 nextVelocity = velocity + gravity * _timeStep;

            // Posición estimada con gravedad y drag
            Vector3 nextPosition = currentPosition + velocity * _timeStep + 0.5f * gravity * (_timeStep * _timeStep);
            Vector3 segment = nextPosition - currentPosition;

            // Comprobar colisión
            if (Physics.Raycast(currentPosition, segment.normalized, out RaycastHit hit, segment.magnitude, _collisionMask))
            {
                points[i] = hit.point;
                i++;
                break;
            }

            points[i] = nextPosition;
            currentPosition = nextPosition;
            velocity = nextVelocity;
        }

        _lineRenderer.positionCount = i;
        _lineRenderer.SetPositions(points);
    }
}
