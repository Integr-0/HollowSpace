using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    private void Start()
    {
        DialogueSystem dialogueSystem = FindObjectOfType<DialogueSystem>();
        dialogueSystem.StartDialogue(new Dialogue
        {
            name = "Test",
            sentences = new string[]
            {
                "Hello, this is a test dialogue.",
                "This is the second sentence.",
                "This is the third sentence.",
                "This is the fourth sentence.",
                "This is the fifth sentence."
            }
        });
    }
}
