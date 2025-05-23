using UnityEngine;
using NuevoInteractor;

public class ButtonStasisDesactivate : MonoBehaviour, IInteractable
{
    [SerializeField] private NewPhysicsBox physicsBox;
    
    private void DesactivateStasis()
    {
        if (physicsBox.isFreezed)
        {
            physicsBox.StatisEffectDeactivate();
        }
    }

    public void Interact()
    {
        DesactivateStasis();
    }
}