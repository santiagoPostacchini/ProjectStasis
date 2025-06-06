using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToCheckpoint : MonoBehaviour
{
    public Transform pos;
    [SerializeField]private GameObject father;
   
    private void OnTriggerEnter(Collider other)
    {
        Player.Player player = other.gameObject.GetComponent<Player.Player>();
        RotateObject fatherMove = father.GetComponent<RotateObject>();
        if (fatherMove != null)
        {
            if (fatherMove.canRotate)
            {
                if (player != null)
                {
                    player.gameObject.transform.position = pos.position;
                }
            }
        }
    }
}
