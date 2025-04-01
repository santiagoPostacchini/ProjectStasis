using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public bool IsHittingRB = false;
    public bool IsHittingNOTRB = false;

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.collider.GetComponent<Rigidbody>())
        {
            IsHittingRB = true;
            IsHittingNOTRB = false;
        }
        else
        {
            IsHittingRB = false;
            IsHittingNOTRB = true;
        }
        var ButtonInteract = hit.gameObject.GetComponent<ButtonPrefab>();
        if (ButtonInteract != null)
        {
            ButtonInteract.InteractionEnter(true);
        }
    }

    private void OnCollisionExit(Collision hit)
    {
        if (hit.collider.GetComponent<Rigidbody>())
        {
            IsHittingRB = false;
        }
        else
        {
            IsHittingNOTRB = false;
        }
        var ButtonInteract = hit.gameObject.GetComponent<ButtonPrefab>();
        if (ButtonInteract != null)
        {
            ButtonInteract.InteractionEnter(false);
        }
    }
}
