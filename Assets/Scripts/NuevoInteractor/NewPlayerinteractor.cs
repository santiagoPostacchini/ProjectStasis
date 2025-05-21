using UnityEngine;

namespace NuevoInteractor
{
    public class NewPlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float pickUpRange = 4f;
        [SerializeField] private float throwForce = 10f;
        [SerializeField] private Transform objectGrabPointTransform;
        [SerializeField] private Transform objectGrabPointBackTransform;

        [Header("Grab System")]
        [SerializeField] private float minHoldDistance = -0.1f;
        [SerializeField] private float maxHoldDistance = 1.5f;
        [SerializeField] private float holderOffset = 0.05f;

        [SerializeField] private Player.Player player;

        private NewPhysicsBox _objectGrabbable;
        
        [Header("Environment")]
        [SerializeField] private LayerMask environmentMask;

        void Update()
        {
            // Raycast para detectar objeto a agarrar
            GameObject hitObject = null;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
            {
                hitObject = hit.collider.gameObject;
            }

            // Agarre y Suelta
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

            // Actualiza posición y rotación del grab point si tenés un objeto agarrado
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
            // 1. Raycast hacia adelante contra environmentMask
            float targetDistance = maxHoldDistance;
            Vector3 surfaceNormal = Vector3.up;    // Por defecto, suelo
            bool hitEnvironment = Physics.Raycast(
                transform.position, 
                transform.forward, 
                out RaycastHit hit, 
                maxHoldDistance, 
                environmentMask
            );

            if (hitEnvironment)
            {
                // Si hay pared/suelo cerca, acorta la distancia para no atravesar
                targetDistance = Mathf.Clamp(hit.distance - holderOffset, minHoldDistance, maxHoldDistance);
                surfaceNormal = hit.normal;  // Normal de la superficie
            }
            // Calcula posición final en el eje forward de la cámara
            Vector3 worldPos = transform.position + transform.forward * targetDistance;

            // 2. Intercala entre posición frontal y backHolder si lo deseas “a mano”:
            //    si quieres directamente saltar a backHolder (preconfigurado), podrías hacer:
            // if (hitEnvironment && hit.distance < someThreshold) {
            //     worldPos = objectGrabPointBackTransform.position;
            // }

            // 3. Calcula la rotación: que la “up” del cubo coincida con surfaceNormal,
            //    y que su “forward” mire hacia la cámara/jugador.
            Vector3 dirToPlayer = (transform.position - worldPos).normalized;
            Quaternion desiredRot = Quaternion.LookRotation(dirToPlayer, surfaceNormal);

            // 4. Aplica al holder y al objeto mismo
            objectGrabPointTransform.SetPositionAndRotation(worldPos, desiredRot);
            _objectGrabbable.transform.SetPositionAndRotation(worldPos, desiredRot);
        }


        public bool HasObjectInHand() => _objectGrabbable;
    }
}
