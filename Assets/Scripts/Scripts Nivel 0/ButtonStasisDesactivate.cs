using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStasisDesactivate : MonoBehaviour,IInteractable
{
    
    [SerializeField] private PhysicsBox physicsBox;
    

    private void DesactivateStasis()
    {
        if (physicsBox._isFreezed)
        {
            physicsBox.StatisEffectDeactivate();
            Debug.Log("Unfreezed");
        }
    }

    public void Interact()
    {
        DesactivateStasis();
    }
}
