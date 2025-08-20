using UnityEngine;
using VIDE_Data;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerInteraction insidePlayerInteraction;
    [SerializeField] private OutsidePlayerInteraction outsidePlayerInteraction;

    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Text npcNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private GameObject continuePrompt; // Assign in inspector


    [Header("Settings")]
    [SerializeField] private float textDisplaySpeed = 0.05f;
    [SerializeField] private float afterDialogueWait = 1.5f;

    private bool isTyping = false;
    private string currentSentence;
    private VIDE_Assign currentDialogue;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(true); // Force active
            VD.LoadDialogues();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Ensure canvas starts disabled
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }
    void Update()
    {
        if (continuePrompt.activeSelf)
        {
            float scale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.1f);
            continuePrompt.transform.localScale = Vector3.one * scale;
        }

        if (VD.isActive && Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue();
        }
    }
    public void StartDialogue(VIDE_Assign dialogue)
    {
        if (!this.enabled) // Double-check
        {
            this.enabled = true;
        }
        gameManager.SetState(GameState.Dialogue);
        currentDialogue = dialogue;
        dialogueCanvas.SetActive(true);
        VD.OnNodeChange += UpdateDialogueUI;
        VD.OnEnd += EndDialogue;
        VD.BeginDialogue(dialogue);

        insidePlayerInteraction?.SetInteractionEnabled(false);
        outsidePlayerInteraction?.SetOutsideInteractionEnabled(false);
    }

    private void UpdateDialogueUI(VD.NodeData data)
    {
        // Clear previous state
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
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
        continuePrompt.SetActive(true); // Show prompt when done typing
    }

    private void SetupChoices(string[] choices)
    {
        choicePanel.SetActive(true);

        foreach (Button button in choiceButtons)
            button.gameObject.SetActive(false);

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

        // Skip typing animation if in progress
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
            continuePrompt.SetActive(true);
            return;
        }

        // Only advance if we're on an NPC node and not showing choices
        if (!VD.nodeData.isPlayer && !VD.nodeData.pausedAction)
        {
            continuePrompt.SetActive(false); // Hide prompt before next line
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
        gameManager.SetState(GameState.Playing);
        dialogueCanvas.SetActive(false);

        //if is NPC, wait 1 second before re-enabling interaction
        var interactable = currentDialogue?.GetComponent<Interactable>();
        if (interactable != null && interactable.IsNPC)
        {
            StartCoroutine(WaitAndEnableInteraction(interactable));
        }
        else
        {
            insidePlayerInteraction?.SetInteractionEnabled(true);
            outsidePlayerInteraction?.SetOutsideInteractionEnabled(true);
        }
    }

    private IEnumerator WaitAndEnableInteraction(Interactable interactable)
    {
        CameraRotation camRotation = FindObjectOfType<CameraRotation>();
        camRotation?.SetCursorLock(enabled);

        yield return new WaitForSeconds(afterDialogueWait);
        insidePlayerInteraction?.SetInteractionEnabled(true);
        outsidePlayerInteraction?.SetOutsideInteractionEnabled(true);
    }

    void OnDestroy()
    {
        VD.OnNodeChange -= UpdateDialogueUI;
        VD.OnEnd -= EndDialogue;
    }
}