using UnityEngine;

namespace NuevoInteractor
{
    public class NewPhysicsBox : MonoBehaviour, IStasis
    {
        [Header("Components")] [SerializeField]
        private Collider mainCollider; // Colisiona con el ambiente SIEMPRE

        [SerializeField] private Collider playerCollider; // Solo colisiona con el jugador
        [SerializeField] private Rigidbody rb;

        private Transform _objGrabPointTransform;
        private Player.Player _player;
        private Vector3 _velocity;

        private bool _isFreezed;

        public Material matStasis;
        private readonly string _outlineThicknessName = "_BorderThickness";
        private MaterialPropertyBlock _mpb;
        private Renderer _renderer;
        private bool _savedKinematic;
        private Vector3 _savedVelocity;
        private Vector3 _savedAngularVelocity;
        private float _savedDrag;

        [SerializeField] private LayerMask originalLayer;

        public bool IsCollidingWithPlayer { get; private set; }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
            SetOutlineThickness(0f);
        }

        public void Grab()
        {
            originalLayer = gameObject.layer;
            gameObject.layer = _objGrabPointTransform.gameObject.layer;
            transform.parent = _objGrabPointTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
            SetPlayerColliderState(true);
        }

        public void Drop()
        {
            transform.parent = null;
            gameObject.layer = originalLayer;
            if (!_isFreezed)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            SetPlayerColliderState(false);
        }

        public void Throw(float force)
        {
            Drop();
            if (!_isFreezed)
            {
                Vector3 throwVelocity = _objGrabPointTransform.forward * (force / rb.mass);
                rb.AddForce(throwVelocity);
            }
        }

        private void SetPlayerColliderState(bool asTrigger)
        {
            playerCollider.isTrigger = asTrigger;
        }

        public void SetReferences(Player.Player assignedPlayer, Transform grabHolder)
        {
            _player = assignedPlayer;
            _objGrabPointTransform = grabHolder;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _player.gameObject)
                IsCollidingWithPlayer = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _player.gameObject)
                IsCollidingWithPlayer = false;
        }

        public void StatisEffectActivate()
        {
            FreezeObject();
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
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                _isFreezed = true;
                SetOutlineThickness(1.05f);
            }
        }

        private void SaveRigidbodyState()
        {
            if (!rb) return;
            _savedKinematic = GetComponent<Rigidbody>().isKinematic;
            _savedVelocity = rb.velocity;
            _savedAngularVelocity = rb.angularVelocity;
            _savedDrag = rb.drag;
        }

        private void RestoreRigidbodyState()
        {
            if (!rb) return;
            rb.velocity = _savedVelocity;
            rb.angularVelocity = _savedAngularVelocity;
            rb.drag = _savedDrag;
            rb.isKinematic = _savedKinematic;
            rb.WakeUp();
        }

        private void UnfreezeObject()
        {
            if (!_isFreezed) return;
            RestoreRigidbodyState();
            _isFreezed = false;
            if (rb)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            SetOutlineThickness(0f);
        }

        private void SetOutlineThickness(float thickness)
        {
            if (!_renderer || _mpb == null) return;
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(_outlineThicknessName, thickness);
            // _mpb.SetColor("_Color", Color.green);
            _renderer.SetPropertyBlock(_mpb);
            //Glow(false, 1);
        }
    }
}