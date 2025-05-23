using System.Collections.Generic;
using NuevoInteractor;
using UnityEngine;

namespace Puzzle_Elements
{
    public class PressurePlate : MonoBehaviour
    {
        private static readonly int IsPressed = Animator.StringToHash("IsPressed");
        
        [Header("Plate Settings")]
        [SerializeField] private PressurePlateGroup plateGroup;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem particles;
        public bool isFrozen;

        public bool IsActivated { get; private set; }
        
        private readonly List<NewPhysicsBox> _physicalBoxes = new List<NewPhysicsBox>();
        private readonly List<NewPhysicsBox> _stasisBoxes   = new List<NewPhysicsBox>();
        
        private void OnCollisionEnter(Collision collision)
        {
            if (isFrozen) return;

            var box = collision.collider.GetComponent<NewPhysicsBox>();
            if (box && !box.isFreezed && !_physicalBoxes.Contains(box))
            {
                _physicalBoxes.Add(box);
                UpdateState();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (isFrozen) return;

            var box = collision.collider.GetComponent<NewPhysicsBox>();
            if (box && !box.isFreezed && _physicalBoxes.Contains(box))
            {
                _physicalBoxes.Remove(box);
                UpdateState();
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (isFrozen) return;
            var box = other.GetComponent<NewPhysicsBox>();
            if (!box || !box.isFreezed || box.transform.parent || _stasisBoxes.Contains(box)) return;
            _stasisBoxes.Add(box);
            UpdateState();
        }

        private void OnTriggerExit(Collider other)
        {
            if (isFrozen) return;
            var box = other.GetComponent<NewPhysicsBox>();
            if (!box || !_stasisBoxes.Contains(box) || (!box.isFreezed && other)) return;
            _stasisBoxes.Remove(box);
            UpdateState();
        }
        
        private void UpdateState()
        {
            bool shouldActivate = _physicalBoxes.Count > 0 || _stasisBoxes.Count > 0;
            if (shouldActivate == IsActivated) return;

            IsActivated = shouldActivate;
            animator?.SetBool(IsPressed, IsActivated);
            plateGroup?.NotifyPlateStateChanged();
        }
        
        public void ActivateEffectParticle()
        {
            particles?.Play();
        }

        public void DesactivateEffectParticle()
        {
            particles?.Stop();
        }
    }
}