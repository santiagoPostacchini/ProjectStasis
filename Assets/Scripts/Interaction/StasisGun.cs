using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public class StasisGun : MonoBehaviour
    {
        [FormerlySerializedAs("_stasisDuration")]
        [Header("Stasis Settings")]
        [SerializeField] private float stasisDuration = 5f;

        [FormerlySerializedAs("_stasisOrigin")]
        [Header("Visual Settings")]
        [SerializeField] private Transform stasisOrigin; // Punto de origen del rayo (por ejemplo, la mano del jugador)
        [SerializeField] private GameObject stasisBeamPrefab; // Prefab del rayo
        [SerializeField] private float beamDuration = 0.2f; // Duración visible del rayo

        private GameObject _firstFrozenObject;
        private IStasis _firstStasisComponent;

        private GameObject _secondFrozenObject;
        private IStasis _secondStasisComponent;

        private StasisBeam _activeBeam; // Instancia activa del rayo

        private Coroutine _stasisTimerCoroutine;
        private Coroutine _beamCoroutine;

        private PlayerInteractor _playerInteractor;
        private bool _isFirstFrozenObjectNotNull;

        private bool IsGunActive => this.enabled && this.gameObject.activeInHierarchy;

        void Start()
        {
            _isFirstFrozenObjectNotNull = _firstFrozenObject;
            _playerInteractor = GetComponent<PlayerInteractor>();
        }

        void Update()
        {
            if (!IsGunActive)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_playerInteractor && _playerInteractor.HasObjectInHand())
                {
                    return;
                }

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

                // Si ya hay un rayo activo, destrúyelo
                if (_activeBeam)
                {
                    Destroy(_activeBeam.gameObject);
                }

                // Instance y configurar el nuevo rayo
                GameObject beamInstance = Instantiate(stasisBeamPrefab, stasisOrigin.position, Quaternion.identity);
                _activeBeam = beamInstance.GetComponent<StasisBeam>();
                _activeBeam.SetBeam(stasisOrigin.position, hit.point);

                //  Reproducir sonido del rayo
                AudioManager.Instance?.PlaySfx("LaserFX");

                // Iniciar la coroutine para desactivar el rayo después de un tiempo
                if (_beamCoroutine != null)
                {
                    StopCoroutine(_beamCoroutine);
                }
                _beamCoroutine = StartCoroutine(DisableBeamAfterDuration(beamDuration));
            }
        }


        void ApplyStasisEffect(GameObject newObject, IStasis newStasisComponent)
        {
            if (!_firstFrozenObject)
            {
                ApplyIndefiniteStasis(newObject, newStasisComponent);
            }
            else if (!_secondFrozenObject && newObject != _firstFrozenObject)
            {
                ApplyTimedStasis(newObject, newStasisComponent);
            }
            else
            {
                UnfreezeObject(newObject);
            }
        }

        void ApplyIndefiniteStasis(GameObject newObject, IStasis newStasisComponent)
        {
            if (_firstFrozenObject && _firstFrozenObject != newObject)
            {
                _firstStasisComponent.StatisEffectDeactivate();
            }

            _firstFrozenObject = newObject;
            _firstStasisComponent = newStasisComponent;
            _firstStasisComponent.StatisEffectActivate();
        }

        void ApplyTimedStasis(GameObject newObject, IStasis newStasisComponent)
        {
            if (_secondFrozenObject == newObject)
            {
                UnfreezeObject(_secondFrozenObject);
                return;
            }

            _secondFrozenObject = newObject;
            _secondStasisComponent = newStasisComponent;
            _secondStasisComponent.StatisEffectActivate();

            if (_stasisTimerCoroutine != null)
            {
                StopCoroutine(_stasisTimerCoroutine);
            }
            _stasisTimerCoroutine = StartCoroutine(UnfreezeBothAfterDelay(stasisDuration));
        }

        void UnfreezeObject(GameObject obj)
        {
            if (obj == _firstFrozenObject)
            {
                _firstStasisComponent.StatisEffectDeactivate();
                _firstFrozenObject = null;
                _firstStasisComponent = null;
            }
            else if (obj == _secondFrozenObject)
            {
                _secondStasisComponent.StatisEffectDeactivate();
                _secondFrozenObject = null;
                _secondStasisComponent = null;
            }
        }

        private IEnumerator UnfreezeBothAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_isFirstFrozenObjectNotNull && _firstStasisComponent != null)
            {
                _firstStasisComponent.StatisEffectDeactivate();
                _firstFrozenObject = null;
                _firstStasisComponent = null;
            }

            if (_secondFrozenObject && _secondStasisComponent != null)
            {
                _secondStasisComponent.StatisEffectDeactivate();
                _secondFrozenObject = null;
                _secondStasisComponent = null;
            }

            _stasisTimerCoroutine = null;
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
            if (_firstStasisComponent != null)
            {
                _firstStasisComponent.StatisEffectDeactivate();
            }
            if (_secondStasisComponent != null)
            {
                _secondStasisComponent.StatisEffectDeactivate();
            }

            _firstFrozenObject = null;
            _secondFrozenObject = null;
            _firstStasisComponent = null;
            _secondStasisComponent = null;
        }
    }
}
