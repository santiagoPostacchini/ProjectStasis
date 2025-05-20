using UnityEngine;

namespace NuevoInteractor
{
    public class NewPhysicsBox : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Collider mainCollider;   // Colisiona con el ambiente SIEMPRE
        [SerializeField] private Collider playerCollider; // Solo colisiona con el jugador
        [SerializeField] private Rigidbody rb;

        private Transform _objGrabPointTransform;
        private Player.Player _player;
        private Vector3 _velocity;

        [Header("Grab Params")]
        [SerializeField] private float followSpeed = 15f;
        [SerializeField] private float rotateSpeed = 15f;

        // Estado de colisión con jugador
        public bool IsCollidingWithPlayer { get; private set; }

        public void Grab(Transform grabPoint)
        {
            _objGrabPointTransform = grabPoint;
            rb.isKinematic = true;
            rb.detectCollisions = true; // Usually you want collisions ON when grabbed, but OFF with the player
            rb.useGravity = false;
            SetPlayerColliderState(true);
        }


        public void Drop()
        {
            _objGrabPointTransform = null;
            _player = null;
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.useGravity = true;
            SetPlayerColliderState(false);
        }


        public void SetPlayerColliderState(bool asTrigger)
        {
            playerCollider.isTrigger = asTrigger;
        }
        
        public void SetReferences(Player.Player assignedPlayer, Transform grabHolder)
        {
            _player = assignedPlayer;
            _objGrabPointTransform = grabHolder;
        }
        
        // Movimiento hacia el Holder y rotación automática
        

        private void Update()
        {
            if (_objGrabPointTransform)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _objGrabPointTransform.position, ref _velocity, 0.05f);
                
                if (_player)
                {
                    Vector3 toPlayer = (_player.transform.position - transform.position).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(toPlayer);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_objGrabPointTransform && _player && other == _player.GetComponent<Collider>())
                IsCollidingWithPlayer = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_objGrabPointTransform && _player && other == _player.GetComponent<Collider>())
                IsCollidingWithPlayer = false;
        }
    }
}
