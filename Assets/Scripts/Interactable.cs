using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isPickable = false;
    [SerializeField] private bool isNPC = false; // New NPC flag
    public bool IsPickable => isPickable;

    [Header("Dialogue Settings")]
    [SerializeField] private VIDE_Assign dialogueAssign; // Reference to VIDE component
    [SerializeField] private string message;

    [Header("Events")]
    [SerializeField] private UnityEvent onInteraction;

    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        DisableOutline();

        // Auto-get VIDE component if NPC
        if (isNPC && dialogueAssign == null)
            dialogueAssign = GetComponent<VIDE_Assign>();
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (isNPC && dialogueAssign != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueAssign);
        }
        else
        {
            onInteraction?.Invoke();
        }
    }

    // Add property to check if this is an NPC
    public bool IsNPC => isNPC;

    public void EnableOutline()
    {
        if (outline != null) outline.enabled = true;
    }

    public void DisableOutline()
    {
        if (outline != null) outline.enabled = false;
    }
}
