using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Open()
    {
        animator?.SetBool("IsOpen", true);
    }

    public void Close()
    {
        animator?.SetBool("IsOpen", false);
    }
}