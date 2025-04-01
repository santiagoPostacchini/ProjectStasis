using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<GameObject> ButtonPList = new List<GameObject>();
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        DoorInteract();
    }
    public void DoorInteract()
    {
        _anim.SetBool("canOpen", CanOpenDoor());
    }
    public bool CanOpenDoor()
    {
        foreach (var item in ButtonPList)
        {
            var itemChild = item.GetComponentInChildren<ButtonPrefab>();
            if (!itemChild.isPressed)
            {
                return false;
            }
            
        }
        return true;
    }
}
