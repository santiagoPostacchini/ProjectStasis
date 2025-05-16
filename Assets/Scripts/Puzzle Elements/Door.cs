using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SlidingDoorController doorController;
    private void Start()
    {
        doorController = GetComponentInParent<SlidingDoorController>();
    }
    public void Open()
    {
        doorController.isOpen = true;
    }

    public void Close()
    {
        doorController.isOpen = false;
    }
}