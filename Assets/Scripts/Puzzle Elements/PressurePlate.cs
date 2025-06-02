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
        [SerializeField] private  NewPlayerInteractor _playerInButton = null;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (isFrozen) return;
            var activator = collision.collider.GetComponent<IPlateActivator>();
            if (activator is NewPhysicsBox box)
            {
                if (box && !box.isFreezed && !_physicalBoxes.Contains(box))
                {
                    _physicalBoxes.Add(box);
                    UpdateState();
                }
            }
            
        }

        private void OnCollisionExit(Collision collision)
        {
            if (isFrozen) return;

            var activator = collision.collider.GetComponent<IPlateActivator>();
            if (activator is NewPhysicsBox box)
            {
                if (box && !box.isFreezed && _physicalBoxes.Contains(box))
                {
                    _physicalBoxes.Clear();
                    _stasisBoxes.Clear();
                    UpdateState();
                }
            }
            
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (isFrozen) return;
            var activator = other.GetComponentInChildren<IPlateActivator>();
            if (activator is NewPhysicsBox box)
            {
                Debug.Log("B");
                if (!box || !box.isFreezed || box.transform.parent || _stasisBoxes.Contains(box)) return;
                _stasisBoxes.Add(box);
                UpdateState();
            }
            else if (activator is NewPlayerInteractor player)
            {
                Debug.Log("C");
                _playerInButton = player;
                UpdateState();
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (isFrozen) return;
            var activator = other.GetComponentInChildren<IPlateActivator>();
            if (activator is NewPhysicsBox box)
            {
                if (!box || !_stasisBoxes.Contains(box) || !box.isFreezed) return;
                _stasisBoxes.Clear();
                _physicalBoxes.Clear();
                UpdateState();
            }
            else if (activator is NewPlayerInteractor player)
            {
                _playerInButton = null;
                UpdateState();
            }

        }
        
        private void UpdateState()
        {
            bool shouldActivate = _physicalBoxes.Count > 0 || _stasisBoxes.Count > 0 || _playerInButton != null;
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