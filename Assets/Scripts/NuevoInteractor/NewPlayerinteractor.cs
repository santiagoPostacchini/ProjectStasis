using UnityEngine;

namespace NuevoInteractor
{
    public class NewPlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")] [SerializeField]
        private float pickUpRange = 4f;

        [SerializeField] private float throwForce = 10f;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private Transform objectGrabPointBackTransform;

        [Header("Grab System")] [SerializeField]
        private float minHoldDistance = -0.1f;

        [SerializeField] private float maxHoldDistance = 1.5f;
        [SerializeField] private float holderOffset = 0.05f;

        [SerializeField] private Player.Player player;

        private NewPhysicsBox _objectGrabbable;

        [Header("Environment")] [SerializeField]
        private LayerMask environmentMask;

        [Header("Smoothing")] [SerializeField] private float positionSmoothTime = 0.1f;
        [SerializeField] private float rotationSmoothSpeed = 10f;
        
        [Header("Custom Movement")]
        [SerializeField] private AnimationCurve holdMoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float headDropStartDist = 0.5f;
        [SerializeField] private float maxHeadDrop = 0.5f;

        
        private Vector3 _frontLocalPos;
        private Vector3 _backLocalPos;
        private Vector3 _localSmoothVel;
        private Quaternion _rotationSmoothQuat;
        private Vector3 _positionSmoothVelocity;

        void Start()
        {
            _frontLocalPos       = objectGrabPointTransform.localPosition;      // debería ser (0,0,1.5)
            _backLocalPos        = objectGrabPointBackTransform.localPosition;  // debería ser (0,0,-0.7)
            _rotationSmoothQuat  = objectGrabPointTransform.rotation;
        }
        void Update()
        {
            GameObject hitObject = null;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
            {
                hitObject = hit.collider.gameObject;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!_objectGrabbable)
                {
                    TryGrabObject(hitObject);
                }
                else
                {
                    TryDropObject();
                }
            }
            
            if (_objectGrabbable)
            {
                UpdateHolderPosition();
            }
        }

        private void TryGrabObject(GameObject hitObject)
        {
            if (hitObject && hitObject.TryGetComponent(out NewPhysicsBox physicsObject))
            {
                objectGrabPointTransform.position = hitObject.transform.position;

                physicsObject.SetReferences(player, objectGrabPointTransform);
                physicsObject.Grab();

                _objectGrabbable = physicsObject;
            }
        }

        private void TryDropObject()
        {
            if (_objectGrabbable && !_objectGrabbable.IsCollidingWithPlayer)
            {
                _objectGrabbable.Drop();
                _objectGrabbable = null;
            }
        }

        private void UpdateHolderPosition()
        {
            // 1) Raycast para ajustar targetDistance
            float targetDistance = maxHoldDistance;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxHoldDistance, environmentMask))
            {
                targetDistance = Mathf.Clamp(hit.distance - holderOffset, minHoldDistance, maxHoldDistance);
            }

            // 2) Calcula t puro [0..1] y lo remapeas con tu curve
            float t       = Mathf.InverseLerp(maxHoldDistance, minHoldDistance, targetDistance);
            float curveT  = holdMoveCurve.Evaluate(t);

            // 3) Define front/back en world space
            Vector3 frontPos = transform.position + transform.forward * maxHoldDistance;
            Vector3 backPos  = objectGrabPointBackTransform.position;

            // 4) Interpola usando curveT
            Vector3 desiredPos = Vector3.Lerp(frontPos, backPos, curveT);

            // 5) Si estás muy cerca, aplica “head drop”
            if (targetDistance < headDropStartDist)
            {
                // headT sube de 0 a 1 cuandao targetDistance va de headDropStartDist a 0
                float headT    = Mathf.InverseLerp(headDropStartDist, minHoldDistance, targetDistance);
                float dropAmt  = Mathf.Lerp(0f, maxHeadDrop, headT);
                desiredPos.y  -= dropAmt;
            }

            // 6) Rotación “mira al jugador” con up = Vector3.up
            Vector3 dirToPlayer  = (transform.position - desiredPos).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer, Vector3.up);
            _rotationSmoothQuat  = Quaternion.Slerp(_rotationSmoothQuat, targetRot, Time.deltaTime * rotationSmoothSpeed);

            // 7) Aplica al holder y al cubo
            objectGrabPointTransform.SetPositionAndRotation(desiredPos, _rotationSmoothQuat);
            _objectGrabbable.transform.SetPositionAndRotation(desiredPos, _rotationSmoothQuat);
        }
        
        public bool HasObjectInHand() => _objectGrabbable;
    }
}