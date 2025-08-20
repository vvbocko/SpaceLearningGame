using UnityEngine;
using System;

public class OutsidePlayerInteraction : MonoBehaviour
{
    public static OutsidePlayerInteraction Instance { get; private set; }

    [Header("References")]
    [SerializeField] private OutsidePickUpContoller outsidePickUpController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Carabiner carabiner; // optional direct reference

    [Header("Settings")]
    [SerializeField] private float playerReach = 3f;

    private Interactable currentInteractable;
    private bool interactionEnabled = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (outsidePickUpController == null)
            outsidePickUpController = FindObjectOfType<OutsidePickUpContoller>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (carabiner == null)
            carabiner = FindObjectOfType<Carabiner>();
    }

    public void SetOutsideInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;
        GetComponent<RailMovement>().enabled = enabled;

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
            UpdateCurrentInteractable(detectedInteractable);

        if (interactionEnabled)
            HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // CASE 1: Click a pinned carabiner -> detach & pick up
            if (carabiner != null && carabiner.IsAttached && currentInteractable == carabiner.GetComponent<Interactable>())
            {
                carabiner.TryDetach();
                outsidePickUpController?.TryPickUp(carabiner.GetComponent<Interactable>());
                return;
            }

            // CASE 2: Holding carabiner && clicked on a rail -> pin to the hang point nearest the click
            if (outsidePickUpController != null && outsidePickUpController.IsHoldingSomething()
                && outsidePickUpController.GetHeldObject() == carabiner?.GetComponent<Interactable>())
            {
                // Do a raycast to get the exact hit point and the shelf hit
                if (playerCamera == null) return;

                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
                {
                    ShelfHangZone shelf = hit.collider.GetComponentInParent<ShelfHangZone>();
                    if (shelf != null)
                    {
                        // choose hang point by using the actual hit.point
                        Transform chosenHangPoint = shelf.GetClosestHangPoint(hit.point);
                        if (chosenHangPoint == null)
                        {
                            Debug.LogWarning("[OutsidePlayerInteraction] No hang point found on shelf.");
                            return;
                        }

                        // Stop pickup controller BEFORE attaching so it won't keep moving the object
                        outsidePickUpController.ForceRelease();

                        // Attach the carabiner to the chosen hang point
                        carabiner.AttachToRail(shelf, chosenHangPoint);
                        return;
                    }
                }
            }

            // CASE 3: Normal pickup (pickable objects)
            if (currentInteractable != null && currentInteractable.IsPickable)
            {
                HandlePickup();
                return;
            }

            // CASE 4: Other interactions
            if (currentInteractable != null && !currentInteractable.IsPickable)
            {
                currentInteractable.Interact();
            }
        }

        // Right click: drop
        if (Input.GetMouseButtonDown(1) && outsidePickUpController != null && outsidePickUpController.IsHoldingSomething())
        {
            outsidePickUpController.Drop();
        }
    }

    void HandlePickup()
    {
        if (outsidePickUpController == null) return;
        if (outsidePickUpController.IsHoldingSomething()) outsidePickUpController.Drop();
        outsidePickUpController.TryPickUp(currentInteractable);
    }

    Interactable DetectInteractable()
    {
        if (playerCamera == null) return null;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
                return interactable;
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
