using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject gO;
    public Image fill;
    private bool isBroken;
    public void BrokenFill(bool b)
    {
        isBroken = b;
        _anim.SetBool("isBroken", b);
    }
    public void IsPressed(bool b)
    {
        // Solo evit�s la activaci�n si est� roto, pero permit�s siempre desactivarlo
        if (b && isBroken) return;
        _anim.SetBool("isPressed", b);
    }
    public void ActivateGameObject()
    {
        gO.gameObject.SetActive(true);
    }
    public void DesactivateGamebject()
    {
        gO.gameObject.SetActive(false);
    }
    public void ResetBrokenFill()
    {
        isBroken = false;
        _anim.SetBool("isBroken", false); // si ten�s esa variable tambi�n
    }
    
}

