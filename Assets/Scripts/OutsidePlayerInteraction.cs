using UnityEngine;
using System;

public class OutsidePlayerInteraction : MonoBehaviour
{
    public static OutsidePlayerInteraction Instance { get; private set; }

    [Header("References")]
    [SerializeField] private OutsidePickUpContoller pickupController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Carabiner carabiner; // Dodaj referencjê do Carabiner

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
        // Left click
        if (Input.GetMouseButtonDown(0))
        {
            // CASE 1: Clicking on a pinned carabiner ? pick it up into hand
            if (carabiner != null && carabiner.IsAttached && currentInteractable == carabiner.GetComponent<Interactable>())
            {
                carabiner.TryDetach();
                pickupController.TryPickUp(carabiner.GetComponent<Interactable>());
                return;
            }

            // CASE 2: Holding carabiner & clicking a rail ? pin to rail
            if (pickupController.IsHoldingSomething() && pickupController.GetHeldObject() == carabiner.GetComponent<Interactable>())
            {
                if (currentInteractable != null && currentInteractable.GetComponent<ShelfHangZone>() != null)
                {
                    ShelfHangZone shelf = currentInteractable.GetComponent<ShelfHangZone>();
                    carabiner.TryAttachToClickedRail(shelf);

                    // release from PickupController so it stops moving it
                    pickupController.ForceRelease();
                    return;
                }
            }

            // CASE 3: Normal pickup logic
            if (currentInteractable != null && currentInteractable.IsPickable)
            {
                HandlePickup();
                return;
            }

            // CASE 4: If it's an interactable but not pickable ? interact
            if (currentInteractable != null && !currentInteractable.IsPickable)
            {
                currentInteractable.Interact();
            }
        }

        // Right click ? drop
        if (Input.GetMouseButtonDown(1) && pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
        }
    }

    void HandlePickup()
    {
        if (pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
        }
        pickupController.TryPickUp(currentInteractable);
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