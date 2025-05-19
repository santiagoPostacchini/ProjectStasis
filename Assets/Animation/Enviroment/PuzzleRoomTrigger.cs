using UnityEngine;

public class PuzzleRoomTrigger : MonoBehaviour
{
    [SerializeField] private MachineryAnimator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            animator.PlayAssembly();
    }
}
