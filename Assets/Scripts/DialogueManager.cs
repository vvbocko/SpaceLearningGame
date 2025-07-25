using UnityEngine;
using VIDE_Data;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Text npcNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private GameObject continuePrompt;

    [Header("Settings")]
    [SerializeField] private float textDisplaySpeed = 0.05f;
    [SerializeField] private float autoAdvanceDelay = 3f;

    private bool isTyping = false;
    private string currentSentence;
    private VIDE_Assign currentDialogue;
    private Coroutine typingCoroutine;
    private Coroutine autoAdvanceCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            VD.LoadDialogues();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(VIDE_Assign dialogue)
    {
        currentDialogue = dialogue;
        dialogueCanvas.SetActive(true);
        VD.OnNodeChange += UpdateDialogueUI;
        VD.OnEnd += EndDialogue;
        VD.BeginDialogue(dialogue);
        PlayerInteraction.Instance.SetInteractionEnabled(false);
    }

    private void UpdateDialogueUI(VD.NodeData data)
    {
        // Clear previous state
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);

        isTyping = false;
        choicePanel.SetActive(false);
        continuePrompt.SetActive(false);
        dialogueText.text = "";

        // Handle NPC dialogue
        if (!data.isPlayer)
        {
            npcNameText.text = data.tag.Length > 0 ? data.tag : currentDialogue.gameObject.name;
            currentSentence = data.comments[data.commentIndex];
            typingCoroutine = StartCoroutine(TypeSentence(currentSentence));

            // Set portrait
            npcPortrait.sprite = data.sprite != null ? data.sprite : currentDialogue.defaultNPCSprite;

            // Auto-advance if not paused
            if (!data.pausedAction)
            {
                autoAdvanceCoroutine = StartCoroutine(AutoAdvance());
            }
        }
        // Handle player choices
        else
        {
            SetupChoices(data.comments);
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textDisplaySpeed);
        }

        isTyping = false;
        continuePrompt.SetActive(true);
    }

    private IEnumerator AutoAdvance()
    {
        yield return new WaitUntil(() => !isTyping);
        yield return new WaitForSeconds(autoAdvanceDelay);
        VD.Next();
    }

    private void SetupChoices(string[] choices)
    {
        choicePanel.SetActive(true);

        // Hide all buttons first
        foreach (Button button in choiceButtons)
            button.gameObject.SetActive(false);

        // Setup available choices
        for (int i = 0; i < choices.Length && i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            Text buttonText = choiceButtons[i].GetComponentInChildren<Text>();
            buttonText.text = choices[i];

            int choiceIndex = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
        }
    }

    public void AdvanceDialogue()
    {
        if (!VD.isActive) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
            continuePrompt.SetActive(true);
            return;
        }

        if (!VD.nodeData.isPlayer && !VD.nodeData.pausedAction)
        {
            if (autoAdvanceCoroutine != null) StopCoroutine(autoAdvanceCoroutine);
            VD.Next();
        }
    }

    private void SelectChoice(int choiceIndex)
    {
        VD.nodeData.commentIndex = choiceIndex;
        VD.Next();
    }

    private void EndDialogue(VD.NodeData data)
    {
        VD.OnNodeChange -= UpdateDialogueUI;
        VD.OnEnd -= EndDialogue;
        VD.EndDialogue();
        dialogueCanvas.SetActive(false);
        PlayerInteraction.Instance.SetInteractionEnabled(true);
    }

    void Update()
    {
        if (VD.isActive && Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue(); // Handles all click-to-continue logic
        }
    }

    void OnDestroy()
    {
        VD.OnNodeChange -= UpdateDialogueUI;
        VD.OnEnd -= EndDialogue;
    }
}