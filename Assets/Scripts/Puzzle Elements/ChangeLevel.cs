using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] posLevel = new Transform[5];
    [SerializeField] private GameObject[] playerAndShadows = new GameObject[3];
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int num = 0;
            UpdatePos(num);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int num = 1;
            UpdatePos(num);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int num = 2;
            UpdatePos(num);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int num = 3;
            UpdatePos(num);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            int num = 4;
            UpdatePos(num);
        }
    }
    private void UpdatePos(int num)
    {
        if (posLevel[num] == null) return;
        foreach (var item in playerAndShadows)
        {
            Vector3 pos = posLevel[num].transform.position;
            item.transform.position = new Vector3(pos.x, item.transform.position.y, pos.z);
        }
    }
}
