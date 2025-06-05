using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToCheckpoint : MonoBehaviour
{
    public Transform pos;

    private void OnCollisionEnter(Collision collision)
    {
        Player.Player player = collision.gameObject.GetComponent<Player.Player>();
        if(player != null)
        {
            player.gameObject.transform.position = pos.position;
        }
    }
}
