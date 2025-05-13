using System.Collections;
using UnityEngine;

public class StasisGun : MonoBehaviour
{
    [Header("Stasis Settings")]
    [SerializeField] private float _stasisRange = 8f;
    [SerializeField] private float _stasisDuration = 5f;

    [Header("Visual Settings")]
    [SerializeField] private Transform _stasisOrigin; // Punto de origen del rayo (por ejemplo, la mano del jugador)
    [SerializeField] private GameObject _stasisBeamPrefab; // Prefab del rayo
    [SerializeField] private float _beamDuration = 0.2f; // Duración visible del rayo

    private GameObject _firstFrozenObject;
    private IStasis _firstStasisComponent;

    private GameObject _secondFrozenObject;
    private IStasis _secondStasisComponent;

    private StasisBeam _activeBeam; // Instancia activa del rayo

    private Coroutine _stasisTimerCoroutine;
    private Coroutine _beamCoroutine;

    private PlayerInteractor _playerInteractor;

    private bool IsGunActive => this.enabled && this.gameObject.activeInHierarchy;

    void Start()
    {
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
            if (_playerInteractor != null && _playerInteractor.HasObjectInHand())
            {
                return;
            }

            TryApplyStasis(Camera.main.transform);
        }
    }

    public void TryApplyStasis(Transform playerCameraTransform)
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, _stasisRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<IStasis>(out IStasis stasisComponent))
            {
                ApplyStasisEffect(hitObject, stasisComponent);
            }

            // Si ya hay un rayo activo, destrúyelo
            if (_activeBeam != null)
            {
                Destroy(_activeBeam.gameObject);
            }

            // Instanciar y configurar el nuevo rayo
            GameObject beamInstance = Instantiate(_stasisBeamPrefab, _stasisOrigin.position, Quaternion.identity);
            _activeBeam = beamInstance.GetComponent<StasisBeam>();
            _activeBeam.SetBeam(_stasisOrigin.position, hit.point);

            // Iniciar la corrutina para desactivar el rayo después de un tiempo
            if (_beamCoroutine != null)
            {
                StopCoroutine(_beamCoroutine);
            }
            _beamCoroutine = StartCoroutine(DisableBeamAfterDuration(_beamDuration));
        }
    }

    void ApplyStasisEffect(GameObject newObject, IStasis newStasisComponent)
    {
        if (_firstFrozenObject == null)
        {
            ApplyIndefiniteStasis(newObject, newStasisComponent);
        }
        else if (_secondFrozenObject == null && newObject != _firstFrozenObject)
        {
            ApplyTimedStasis(newObject, newStasisComponent);
        }
        else
        {
            UnfreezeObject(newObject, newStasisComponent);
        }
    }

    void ApplyIndefiniteStasis(GameObject newObject, IStasis newStasisComponent)
    {
        if (_firstFrozenObject != null && _firstFrozenObject != newObject)
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
            UnfreezeObject(_secondFrozenObject, _secondStasisComponent);
            return;
        }

        _secondFrozenObject = newObject;
        _secondStasisComponent = newStasisComponent;
        _secondStasisComponent.StatisEffectActivate();

        if (_stasisTimerCoroutine != null)
        {
            StopCoroutine(_stasisTimerCoroutine);
        }
        _stasisTimerCoroutine = StartCoroutine(UnfreezeBothAfterDelay(_stasisDuration));
    }

    void UnfreezeObject(GameObject obj, IStasis stasisComponent)
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

        if (_firstFrozenObject != null && _firstStasisComponent != null)
        {
            _firstStasisComponent.StatisEffectDeactivate();
            _firstFrozenObject = null;
            _firstStasisComponent = null;
        }

        if (_secondFrozenObject != null && _secondStasisComponent != null)
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

        if (_activeBeam != null)
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
