using NuevoInteractor;
using UnityEngine;

namespace Puzzle_Elements
{
    public class CubePodium : MonoBehaviour, IInteractable
    {
        public NewPhysicsBox box;
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
        public void SuspendObject()
        {
            if (!box) return;
            Rigidbody rb = box.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = false;
                box.transform.position = t.transform.position;
                Rotator boxRotator = box.GetComponent<Rotator>();
                if(boxRotator != null)
                {
                    boxRotator.canRotate = true;
                }
            }
            particle?.Play();

        }

        public void Interact()
        {
            particle?.Stop();
            Rotator boxRotator = box.GetComponent<Rotator>();
            if (boxRotator != null)
            {
                boxRotator.canRotate = false;
            }
        }
    }
}
