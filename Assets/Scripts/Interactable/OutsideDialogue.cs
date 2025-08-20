using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideDialogue : MonoBehaviour
{
    [SerializeField] private VIDE_Assign dialogueAssign;
    [SerializeField] private GameObject player;
    [SerializeField] private BoxCollider dialogueCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerOutside") && dialogueAssign != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueAssign);
            dialogueCollider.enabled = false;
        }
    }
}
