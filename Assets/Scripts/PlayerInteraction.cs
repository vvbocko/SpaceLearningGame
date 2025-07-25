using UnityEngine;
using System;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PickupController pickupController;
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float playerReach = 3f;

    private Interactable currentInteractable;
    private bool interactionEnabled = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;
        GetComponent<ZeroGravityMovement>().enabled = enabled;

        CameraRotation camRotation = FindObjectOfType<CameraRotation>();
        camRotation?.SetCursorLock(enabled);

        if (!enabled && currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }

    void Update()
    {
        HandleInteraction();
        // ▼ Removed the dialogue progression check entirely ▼
        // Dialogue progression is now fully handled by DialogueManager's Update()
    }

    void HandleInteraction()
    {
        Interactable detectedInteractable = DetectInteractable();

        if (detectedInteractable != currentInteractable)
        {
            UpdateCurrentInteractable(detectedInteractable);
        }

        if (interactionEnabled) HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1) && pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
            return;
        }

        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            if (currentInteractable.IsPickable)
            {
                HandlePickup();
            }
            else
            {
                currentInteractable.Interact();
            }
        }
    }

    void HandlePickup()
    {
        if (pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
        }
        pickupController.TryPickup(currentInteractable);
    }

    Interactable DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                return interactable;
            }
                
        }
        return null;
    }

    void UpdateCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable?.DisableOutline();
        currentInteractable = newInteractable;
        currentInteractable?.EnableOutline();
    }
}