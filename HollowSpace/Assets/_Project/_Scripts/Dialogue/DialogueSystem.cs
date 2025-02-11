using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField, Tooltip("How long to wait to type the next letter (in ms)")] 
    private float msToNextLetter = 20f;
    
    public bool IsDialogueActive => dialoguePanel.activeSelf;
    
    private Queue<string> _sentences;
    private bool _isTyping;
    
    private void Awake()
    {
        _sentences = new Queue<string>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isTyping)
        {
            DisplayNextSentence();
        }
    }
    
    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        nameText.text = dialogue.name;
        _sentences.Clear();
        
        foreach (var sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        
        DisplayNextSentence();
    }
    
    private void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = _sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    
    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        dialogueText.text = "";
        
        foreach (var letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(msToNextLetter / 1000);
        }
        
        _isTyping = false;
    }
    
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
    
    public void SkipDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        _isTyping = false;
        EndDialogue();
    }
}
