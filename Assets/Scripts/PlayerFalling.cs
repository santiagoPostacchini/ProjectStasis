using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling : MonoBehaviour
{
    [SerializeField] private Transform t;



    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if(player != null)
        {
            player.gameObject.transform.position = t.position;
        }
    }
}
