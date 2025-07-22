using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private bool isPickable = false;
    public bool IsPickable => isPickable;

    [SerializeField] private string message;
    [SerializeField] private UnityEvent onInteraction;

    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        DisableOutline();
    }

    public void Interact()
    {
        if (isInteractable)
        {
            onInteraction?.Invoke();
        }
    }

    public void EnableOutline()
    {
        if (outline != null) outline.enabled = true;
    }

    public void DisableOutline()
    {
        if (outline != null) outline.enabled = false;
    }
}
