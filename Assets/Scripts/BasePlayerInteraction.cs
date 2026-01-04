using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Camera playerCamera;
    [Header("Settings")]
    [SerializeField] protected float playerReach = 3f;

    protected Interactable currentInteractable;
    protected bool interactionEnabled = true;

    protected virtual void Awake() { }

    public virtual void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;
        CameraRotation camRotation = FindObjectOfType<CameraRotation>();
        camRotation?.SetCursorLock(enabled);

        if (!enabled && currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }

    protected virtual void Update()
    {
        HandleInteraction();
    }

    protected virtual void HandleInteraction()
    {
        Interactable detectedInteractable = DetectInteractable();
        if (detectedInteractable != currentInteractable)
        {
            UpdateCurrentInteractable(detectedInteractable);
        }
        if (interactionEnabled) HandleInput();
    }

    protected abstract void HandleInput();

    protected abstract Interactable DetectInteractable();

    protected virtual void UpdateCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable?.DisableOutline();
        currentInteractable = newInteractable;
        currentInteractable?.EnableOutline();
    }
}
