using UnityEngine;

namespace NuevoInteractor
{
    public class NewPhysicsBox : MonoBehaviour, IStasis
    {
        [Header("Components")] [SerializeField]
        private Collider mainCollider; // Colisiona con el ambiente SIEMPRE

        [SerializeField] private Collider playerCollider; // Solo colisiona con el jugador
        public Rigidbody rb;

        private Transform _objGrabPointTransform;
        private Player.Player _player;
        private Vector3 _velocity;

        public bool isFreezed;

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

        [SerializeField] private TrajectoryCube trajectoryCube;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
            trajectoryCube = GetComponent<TrajectoryCube>();
            SetOutlineThickness(0f);
        }

        public void Grab()
        {
            if (!isFreezed)
            {
                _savedVelocity = Vector3.zero;
                _savedAngularVelocity = Vector3.zero;
            }

            originalLayer = gameObject.layer;
            gameObject.layer = _objGrabPointTransform.gameObject.layer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = _objGrabPointTransform.gameObject.layer;
            }

            transform.parent = _objGrabPointTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
            trajectoryCube.lineRenderer.positionCount = 0;
            SetPlayerColliderState(true);
        }

        public void Drop()
        {
            transform.parent = null;
            gameObject.layer = originalLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = originalLayer;
            }

            if (!isFreezed)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            else
            {
                float speed = _savedVelocity.magnitude;
                _savedVelocity = -_objGrabPointTransform.forward * speed;

                if (_savedVelocity.magnitude > 0.5f)
                {
                    trajectoryCube.DrawTrajectory(transform.position, _savedVelocity, rb.drag);
                }
            }

            SetPlayerColliderState(false);
        }

        public void Throw(float force)
        {
            Drop();
            if (!isFreezed)
            {
                Vector3 throwVelocity = -_objGrabPointTransform.forward * (force / rb.mass);
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
            if (!_player) return;
            if (other.gameObject == _player.gameObject)
                IsCollidingWithPlayer = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_player) return;
            if (other.gameObject == _player.gameObject)
                IsCollidingWithPlayer = false;
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
                SaveRigidbodyState();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
                isFreezed = true;
                if (_savedVelocity.magnitude > 0.5f)
                {
                    trajectoryCube.DrawTrajectory(transform.position, _savedVelocity, rb.drag);
                }

                SetOutlineThickness(1.05f);
            }
        }

        private void SaveRigidbodyState()
        {
            if (!rb) return;
            _savedKinematic = rb.isKinematic;
            _savedVelocity = rb.velocity;
            _savedAngularVelocity = rb.angularVelocity;
            _savedDrag = rb.drag;
        }

        private void RestoreRigidbodyState()
        {
            if (!rb) return;
            rb.isKinematic = _savedKinematic;
            rb.velocity = _savedVelocity;
            rb.angularVelocity = _savedAngularVelocity;
            rb.drag = _savedDrag;
            rb.WakeUp();
        }

        private void UnfreezeObject()
        {
            if (!isFreezed) return;
            RestoreRigidbodyState();
            isFreezed = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            SetOutlineThickness(0f);

            if (trajectoryCube.lineRenderer)
                trajectoryCube.lineRenderer.positionCount = 0;
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