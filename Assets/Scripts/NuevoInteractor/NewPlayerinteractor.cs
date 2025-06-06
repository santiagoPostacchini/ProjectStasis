using Interaction;
using Player;
using UnityEngine;
using Events;

namespace NuevoInteractor
{
    public class NewPlayerInteractor : MonoBehaviour,IPlateActivator
    {
        [Header("Interaction Settings")]
        [SerializeField] private float throwCharge;
        [SerializeField] private float pickUpRange = 4f;
        [SerializeField] private float holdTime;
        [SerializeField] private float throwHoldThreshold = 0.15f;
        public float ThrowCharge => Mathf.Clamp01(throwCharge / throwHoldThreshold);
        private bool _isHoldingThrow;

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

        [Header("Smoothing")]
        [SerializeField] private float rotationSmoothSpeed = 10f;

        [Header("Custom Movement")] [SerializeField]
        private AnimationCurve holdMoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float headDropStartDist = 0.5f;
        [SerializeField] private float maxHeadDrop = 0.5f;
        
        [Header("FX Settings")]
        [SerializeField] private StasisObjectEffects stasisEffects;
        
        private Vector3 _localSmoothVel;
        private Quaternion _rotationSmoothQuat;
        private Vector3 _positionSmoothVelocity;

        void Start()
        {
            _rotationSmoothQuat = objectGrabPointTransform.rotation;
        }

        void Update()
        {
            IStasis lookedStasisObject = null;
            GameObject hitObject = null;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
            {
                hitObject = hit.collider.gameObject;
                
            }
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit1))
            {
                lookedStasisObject = hit1.collider.GetComponent<IStasis>();

            }

           // stasisEffects.HandleVisualStasisFeedback(lookedStasisObject, HasObjectInHand());
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!_objectGrabbable)
                {
                    if (hitObject)
                    {
                        if (hitObject.GetComponent<IInteractable>() != null)
                        {
                            hitObject.GetComponent<IInteractable>().Interact();
                            Debug.Log("B");
                        }
                        else
                        {
                            TryGrabObject(hitObject);
                        }
                    }
                }
                else
                {
                    _isHoldingThrow = true;
                    holdTime = 0f;
                    throwCharge = 0f;
                    
                }
            }
            
            if (_isHoldingThrow && _objectGrabbable)
            {
                holdTime += Time.deltaTime;
                throwCharge = holdTime;
                ThrowUISlider.Instance?.SetFill(Mathf.Clamp01(throwCharge/throwHoldThreshold));
                
                if (holdTime >= throwHoldThreshold)
                {
                    if (_objectGrabbable && !_objectGrabbable.IsOverlappingAnything)
                    {
                        _objectGrabbable.Throw(throwForce);
                        _objectGrabbable = null;
                        _isHoldingThrow = false;
                        holdTime = 0f;
                        throwCharge = 0f;
                        ThrowUISlider.Instance?.SetFill(0);
                        
                        EventManager.TriggerEvent("OnObjectThrow", gameObject);
                    }
                }
            }
            
            if (Input.GetKeyUp(KeyCode.E) && _objectGrabbable && _isHoldingThrow)
            {
                if (holdTime < throwHoldThreshold)
                {
                    TryDropObject();
                }
                _isHoldingThrow = false;
                holdTime = 0f;
                throwCharge = 0f;
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
                physicsObject.SetReferences(objectGrabPointTransform);
                physicsObject.Grab();
                _objectGrabbable = physicsObject;
                
                EventManager.TriggerEvent("OnObjectGrab", gameObject);
            }
        }

        private void TryDropObject()
        {
            if (_objectGrabbable && !_objectGrabbable.IsOverlappingAnything)
            {
                ThrowUISlider.Instance?.SetFill(0);
                _objectGrabbable.Drop();
                _objectGrabbable = null;
                
                EventManager.TriggerEvent("OnObjectDrop", gameObject);
            }
        }

        private void UpdateHolderPosition()
        {
            float targetDistance = maxHoldDistance;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxHoldDistance,
                    environmentMask))
            {
                targetDistance = Mathf.Clamp(hit.distance - holderOffset, minHoldDistance, maxHoldDistance);
            }
            
            float t = Mathf.InverseLerp(maxHoldDistance, minHoldDistance, targetDistance);
            float curveT = holdMoveCurve.Evaluate(t);
            
            Vector3 frontPos = transform.position + transform.forward * maxHoldDistance;
            Vector3 backPos = objectGrabPointBackTransform.position;
            
            Vector3 desiredPos = Vector3.Lerp(frontPos, backPos, curveT);
            
            if (targetDistance < headDropStartDist)
            {
                float headT = Mathf.InverseLerp(headDropStartDist, minHoldDistance, targetDistance);
                float dropAmt = Mathf.Lerp(0f, maxHeadDrop, headT);
                desiredPos.y -= dropAmt;
            }
            
            Vector3 dirToPlayer = (transform.position - desiredPos).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer, Vector3.up);
            _rotationSmoothQuat =
                Quaternion.Slerp(_rotationSmoothQuat, targetRot, Time.deltaTime * rotationSmoothSpeed);
            
            objectGrabPointTransform.SetPositionAndRotation(desiredPos, _rotationSmoothQuat);
            _objectGrabbable.transform.SetPositionAndRotation(desiredPos, _rotationSmoothQuat);
        }

        public bool HasObjectInHand() => _objectGrabbable && _objectGrabbable.gameObject.activeInHierarchy;
    }
}