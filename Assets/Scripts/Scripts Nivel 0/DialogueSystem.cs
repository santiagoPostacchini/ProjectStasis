using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Config")]
    public float typingSpeed = 0.05f;
    public float delayBetweenLines = 1.5f;

    private string[] lines;
    private int currentLine;
    private List<UnityAction> onDialogueEndActions;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Inicia el diálogo con múltiples acciones que se ejecutan al finalizar.
    /// </summary>
    public void StartDialogue(string[] dialogueLines, params UnityAction[] endActions)
    {
        lines = dialogueLines;
        currentLine = 0;
        onDialogueEndActions = new List<UnityAction>(endActions);

        dialoguePanel.SetActive(true);
        StartTypingLine();
    }

    void StartTypingLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(delayBetweenLines);

        currentLine++;

        if (currentLine < lines.Length)
        {
            StartTypingLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (onDialogueEndActions != null)
        {
            foreach (var action in onDialogueEndActions)
            {
                action?.Invoke();
            }
        }
    }
}