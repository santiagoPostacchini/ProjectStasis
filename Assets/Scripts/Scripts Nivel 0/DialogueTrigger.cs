using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea] public string[] lines;
    public UnityEvent onDialogueComplete;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered && other.CompareTag("Player"))
        {
            alreadyTriggered = true;

            // Convertir UnityEvent a UnityAction
            DialogueSystem.Instance.StartDialogue(
                lines,
                onDialogueComplete.Invoke
            );
        }
    }
}