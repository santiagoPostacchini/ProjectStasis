using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    [SerializeField]private GameObject box;
    [SerializeField]private Transform posRespawn;
    
    public int amount = 1;
    
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            GenerateCube();
        }
    }

    public void GenerateCube()
    {
        box.transform.position = posRespawn.position;

    }
}
