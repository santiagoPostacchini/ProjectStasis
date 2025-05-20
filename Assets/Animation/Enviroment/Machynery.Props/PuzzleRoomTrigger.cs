using UnityEngine;

public class PuzzleRoomTrigger : MonoBehaviour
{
    [SerializeField] private Animator machineryAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        machineryAnimator.SetTrigger("DoAssemble");
    }
}
