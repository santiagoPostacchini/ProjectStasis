using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform pos3;
    [SerializeField] private Transform pos4;
    [SerializeField] private Player.Player player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player == null) return;
            player.transform.position = pos1.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (player == null) return;
            player.transform.position = pos2.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (player == null) return;
            player.transform.position = pos3.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (player == null) return;
            player.transform.position = pos4.transform.position;
        }
    }
}
