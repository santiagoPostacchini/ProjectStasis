using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    [SerializeField] private Transform pos;
    [SerializeField] private Player player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player == null) return;
            player.transform.position = pos.transform.position;
        }
    }
}
