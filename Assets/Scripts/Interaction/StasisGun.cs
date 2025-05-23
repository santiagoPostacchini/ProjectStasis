using System.Collections;
using NuevoInteractor;
using UnityEngine;

namespace Interaction
{
    public class StasisGun : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Transform stasisOrigin;
        [SerializeField] private GameObject stasisBeamPrefab;
        [SerializeField] private float beamDuration = 0.2f;

        private GameObject _firstFrozenObject;
        private IStasis _firstStasisComponent;

        private GameObject _secondFrozenObject;
        private IStasis _secondStasisComponent;

        private StasisBeam _activeBeam;
        private Coroutine _beamCoroutine;

        private NewPlayerInteractor _playerInteractor;

        private bool IsGunActive => this.enabled && this.gameObject.activeInHierarchy;

        void Start()
        {
            _playerInteractor = GetComponent<NewPlayerInteractor>();
        }

        void Update()
        {
            if (!IsGunActive)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_playerInteractor && _playerInteractor.HasObjectInHand())
                    return;

                TryApplyStasis(Camera.main?.transform);
            }
        }

        private void TryApplyStasis(Transform playerCameraTransform)
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.TryGetComponent<IStasis>(out var stasisComponent))
                {
                    ApplyStasisEffect(hitObject, stasisComponent);
                }
                
                if (_activeBeam)
                {
                    Destroy(_activeBeam.gameObject);
                }

                GameObject beamInstance = Instantiate(stasisBeamPrefab, stasisOrigin.position, Quaternion.identity);
                _activeBeam = beamInstance.GetComponent<StasisBeam>();
                _activeBeam.SetBeam(stasisOrigin.position, hit.point);

                AudioManager.Instance?.PlaySfx("LaserFX");

                if (_beamCoroutine != null)
                    StopCoroutine(_beamCoroutine);

                _beamCoroutine = StartCoroutine(DisableBeamAfterDuration(beamDuration));
            }
        }

        void ApplyStasisEffect(GameObject newObject, IStasis newStasisComponent)
        {
            if (newObject == _firstFrozenObject)
            {
                _firstStasisComponent.StatisEffectDeactivate();
                _firstFrozenObject = null;
                _firstStasisComponent = null;
                return;
            }
            if (newObject == _secondFrozenObject)
            {
                _secondStasisComponent.StatisEffectDeactivate();
                _secondFrozenObject = null;
                _secondStasisComponent = null;
                return;
            }
            
            if (_firstFrozenObject && _secondFrozenObject)
            {
                _firstStasisComponent.StatisEffectDeactivate();
                
                _firstFrozenObject = _secondFrozenObject;
                _firstStasisComponent = _secondStasisComponent;
                _secondFrozenObject = null;
                _secondStasisComponent = null;
            }

            // Si hay lugar en el segundo slot, poner el nuevo ahí
            if (!_firstFrozenObject)
            {
                _firstFrozenObject = newObject;
                _firstStasisComponent = newStasisComponent;
                _firstStasisComponent.StatisEffectActivate();
            }
            else if (!_secondFrozenObject)
            {
                _secondFrozenObject = newObject;
                _secondStasisComponent = newStasisComponent;
                _secondStasisComponent.StatisEffectActivate();
            }
        }

        private IEnumerator DisableBeamAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (_activeBeam)
            {
                Destroy(_activeBeam.gameObject);
                _activeBeam = null;
            }
        }

        private void OnDisable()
        {
            UnfreezeAllObjects();
        }

        private void UnfreezeAllObjects()
        {
            _firstStasisComponent?.StatisEffectDeactivate();
            _secondStasisComponent?.StatisEffectDeactivate();

            _firstFrozenObject = null;
            _secondFrozenObject = null;
            _firstStasisComponent = null;
            _secondStasisComponent = null;
        }
    }
}