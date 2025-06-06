using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerAndArranger : MonoBehaviour
{
    public Transform targetPosition; // La posición final hacia la que se moverá
    private Vector3 posInitial;
    public float speed = 5f;         // Velocidad del movimiento
    private void Start()
    {
        posInitial = transform.position;
    }
    private void Update()
    {
        if (targetPosition != null)
        {
            if (!ChildreenOn()) return;
            if(Vector3.Distance(posInitial,targetPosition.position) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            }
            
        }
    }
    private bool ChildreenOn()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                return false;
            }
         
        }
        return true;
    }
}
