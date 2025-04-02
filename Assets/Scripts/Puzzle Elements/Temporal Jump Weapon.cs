using System.Collections;
using UnityEngine;

public class TemporalJumpWeapon : MonoBehaviour
{
    [SerializeField] private Transform _futureShadow;
    [SerializeField] private Transform _pastShadow;
    [SerializeField] private bool _inFuture = true;
    private bool _canTeleport = true;
    private bool _isTeleporting = false;
    private PlayerInteractor playerInteractor;

    void Start()
    {
        playerInteractor = GetComponentInChildren<PlayerInteractor>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && _canTeleport)
        {
            CleanPastObject();
            StartCoroutine(TemporalJump());
        }
    }

    void CleanPastObject()
    {
        if (playerInteractor?._objectGrabbable != null)
        {
            if (playerInteractor._objectGrabbable.GetComponent<ObjectUpdater>() != null)
            {
                playerInteractor._objectGrabbable.Drop();
                playerInteractor.ClearHands();
            }
        }
    }

    IEnumerator TemporalJump()
    {
        _canTeleport = false;

        ShadowPlayer futureSP = _futureShadow.GetComponent<ShadowPlayer>();
        if (futureSP != null)
        {
            futureSP.UpdateShadowPlayerPosition();
        }
        ShadowPlayer pastSP = _pastShadow.GetComponent<ShadowPlayer>();
        if (pastSP != null)
        {
            pastSP.UpdateShadowPlayerPosition();
        }

        if (_inFuture)
        {
            transform.position = _pastShadow.position;
        }
        else
        {
            transform.position = _futureShadow.position;
        }

        _inFuture = !_inFuture;

        yield return new WaitForEndOfFrame();

        _canTeleport = true;
    }
}
