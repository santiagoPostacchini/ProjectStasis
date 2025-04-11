using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleObject : MonoBehaviour
{
    [SerializeField] private Door door;
    private bool alreadyOpen;
    
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<Player>();
        if(player != null)
        {
            //Debug.Log("Esta colisionando");
            alreadyOpen = false;
            StartCoroutine(OpenAndCloseDoor());
        }
    }
    
    IEnumerator OpenAndCloseDoor()
    {
        if (!alreadyOpen)
        {
            door.Open();
            alreadyOpen = true;
        }
        
        yield return new WaitForSeconds(2f);
        door.Close();
        alreadyOpen = false;
    }
}
