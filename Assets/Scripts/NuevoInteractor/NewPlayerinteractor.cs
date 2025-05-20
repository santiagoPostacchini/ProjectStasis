using UnityEngine;
using UnityEngine.Serialization;

namespace NuevoInteractor
{
    public class NewPlayerInteractor : MonoBehaviour
    {
        [FormerlySerializedAs("_pickUpRange")]
        [Header("Interaction Settings")]
        [SerializeField] private float pickUpRange = 4f;
        [SerializeField] private float throwForce = 10f;
        [SerializeField] private Transform _objectGrabPointTransform;
        [SerializeField] private Transform _objectGrabPointBackTransform;

        [Header("Grab System")]
        [SerializeField] private float grabMoveSpeed = 15f;
        [SerializeField] private float grabRotateSpeed = 15f;
        [SerializeField] private float minHoldDistance = -0.1f;
        [SerializeField] private float maxHoldDistance = 1.5f;
        [SerializeField] private float holderOffset = 0.05f;

        public NewPhysicsBox objectGrabbable;
        private bool _isInteractableInView;
        [SerializeField] private Player.Player player;
        [SerializeField] private LayerMask environmentMask;
        
        void Update()
        {
            // --- Interacción de Raycast ---
            GameObject hitObject = null;
            bool hitInteractable = false;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
            {
                hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("Interactable"))
                    hitInteractable = true;
            }

            if (hitInteractable != _isInteractableInView && !objectGrabbable)
            {
                _isInteractableInView = hitInteractable;
                CrosshairController.Instance.ToggleCrosshairSize(_isInteractableInView);
            }

            // --- Agarre y Lanzamiento ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!objectGrabbable)
                {
                    TryGrabObject(hitObject);
                }
                else
                {
                    TryDropObject();
                }
            }

            // --- Mantener el objeto agarrado y ajustar el Holder ---
            if (objectGrabbable)
            {
                UpdateHolderPosition();
            }
        }

        private void TryGrabObject(GameObject hitObject)
        {
            if (hitObject && hitObject.TryGetComponent(out NewPhysicsBox physicsObject))
            {
                objectGrabbable = physicsObject;
                objectGrabbable.SetReferences(player, _objectGrabPointTransform);
                objectGrabbable.Grab(_objectGrabPointTransform);
                objectGrabbable.SetPlayerColliderState(true);
            }
        }

        private void TryDropObject()
        {
            if (objectGrabbable)
            {
                if (objectGrabbable.IsCollidingWithPlayer)
                    return;

                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }


        private void UpdateHolderPosition()
        {
            float targetDistance = maxHoldDistance;

            // Raycast desde la cámara hacia adelante, solo al entorno, ignorando el cubo agarrado
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxHoldDistance, environmentMask))
            {
                targetDistance = Mathf.Clamp(hit.distance - holderOffset, minHoldDistance, maxHoldDistance);
            }

            // Calcula el "progreso" entre la distancia mínima y máxima (0 = cerca, 1 = lejos)
            float t = Mathf.InverseLerp(minHoldDistance, maxHoldDistance, targetDistance);

            // Interpola entre la posición mínima y la máxima del holder
            Vector3 minHolderPos = _objectGrabPointBackTransform.position;
            Vector3 maxHolderPos = transform.position + transform.forward * maxHoldDistance;
            Vector3 targetPos = Vector3.Lerp(minHolderPos, maxHolderPos, t);

            _objectGrabPointTransform.position = targetPos;
            _objectGrabPointTransform.rotation = Quaternion.LookRotation(transform.forward);
        }

        public bool HasObjectInHand() => objectGrabbable;
    }
}
