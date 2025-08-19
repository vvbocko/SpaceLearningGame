using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Interactable : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameManager gameManager;

    [Header("Interaction Settings")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isPickable = false;
    [SerializeField] public bool isNPC = false; // NPC flag
    public bool IsPickable => isPickable;

    [Header("Dialogue Settings")]
    [SerializeField] private VIDE_Assign dialogueAssign; // Reference to VIDE component
    [SerializeField] private Transform player;
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
            gameManager.UpdateAstronautProgressFromNPC(this);

            // Start smooth look at player
            StopAllCoroutines();
            StartCoroutine(LookAtPlayerCoroutine(player));
        }
        else
        {
            onInteraction?.Invoke();
        }
    }

    private IEnumerator LookAtPlayerCoroutine(Transform target)
    {
        float duration = 0.5f; // time to rotate
        float elapsed = 0f;

        Quaternion startRot = transform.rotation;
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // keep only horizontal rotation, remove tilt
        Quaternion targetRot = Quaternion.LookRotation(direction);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            yield return null;
        }

        transform.rotation = targetRot; // snap at the end just in case
    }

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
