using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClimbIt : MonoBehaviour
{
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NoClimb"))
        {
            PlayerController2 player = other.GetComponentInParent<PlayerController2>();
            if(player != null)
            {
             //   player.climbBlockTrigger = true;
             //
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoClimb"))
        {
            PlayerController2 player = other.GetComponentInParent<PlayerController2>();
            if (player != null)
            {
               // player.climbBlockTrigger = false;
            }
        }
        if (other.CompareTag("CanAir"))
        {
            PlayerController2 player = other.GetComponentInParent<PlayerController2>();
            if (player != null)
            {
                //player.iAMClimbing = false;
            }
        }
    }
}
