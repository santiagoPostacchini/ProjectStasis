using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPrefab : MonoBehaviour
{
    public bool isPressed = false;
    public bool isFrozen = false;
    public PhysicsObject pO;
    private Animator _anim;
    private void Start()
    {
        _anim = GetComponentInParent<Animator>();
        pO = GetComponent<PhysicsObject>();
        isPressed = false;
    }
    private void Update()
    {
        if(isFrozen != pO.isFrozen)
        {
            InteractionEnter(pO.isFrozen);
            isFrozen = pO.isFrozen;
        }
    }
    public  void InteractionEnter(bool a)
    {
        if (pO.isFrozen) return;
        _anim.SetBool("isPressed", a);
        isPressed = a;

    }
}
