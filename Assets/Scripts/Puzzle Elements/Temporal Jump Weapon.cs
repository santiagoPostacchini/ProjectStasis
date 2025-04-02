using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalJumpWeapon : MonoBehaviour
{
    public Transform futurePosPlayer;
    public Transform auxPosPlayer;
    [SerializeField]private bool canUseTemporalJumpWeapon;
    public bool iAmInTheFuture= true;
    private PlayerInteractor playerInteractor;

    // Start is called before the first frame update
    void Start()
    {
        canUseTemporalJumpWeapon = true;
        playerInteractor = GetComponent<PlayerInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!canUseTemporalJumpWeapon) return;
            
            if(playerInteractor != null)
            {
                if (playerInteractor._objectGrabbable != null)
                {
                    if (playerInteractor._objectGrabbable.TryGetComponent(out ObjectPast op))
                    {
                        playerInteractor._objectGrabbable.Drop();
                        playerInteractor.ClearHands();
                        
                    }
                    
                }
            }
            StartCoroutine(TemporalJump());
        }
    }
    IEnumerator TemporalJump()
    {
        canUseTemporalJumpWeapon = false;
        yield return new WaitForSeconds(0.5f);
        ShadowPlayer sP = futurePosPlayer.GetComponent<ShadowPlayer>();
        if (sP != null)
        {
            sP.UpdateShadowPlayerPosition();
        }
        auxPosPlayer.position = transform.position;
        transform.position = futurePosPlayer.position;
        futurePosPlayer.position = auxPosPlayer.position;

        iAmInTheFuture = !iAmInTheFuture;



        canUseTemporalJumpWeapon = true;

    }
}
