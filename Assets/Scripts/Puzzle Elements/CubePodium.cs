using NuevoInteractor;
using UnityEngine;

namespace Puzzle_Elements
{
    public class CubePodium : MonoBehaviour, IInteractable
    {
        [SerializeField] private NewPhysicsBox box;
        [SerializeField] private Transform t;
        [SerializeField] private ParticleSystem particle;

        // Start is called before the first frame update
        void Start()
        {
            SuspendObject();
            particle.Play();
        }
        private void Update()
        {
            if (!box) return;
            float distance = Vector3.Distance(transform.position, box.transform.position);
            if (distance > 2)
            {
                Interact();
            }
        }
        private void SuspendObject()
        {
            if (!box) return;
            Rigidbody rb = box.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = false;
                box.transform.position = t.transform.position;
            }

        }

        public void Interact()
        {
            particle?.Stop();
        }
    }
}
