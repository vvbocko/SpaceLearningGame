using UnityEngine;
using System;

public class OutsidePlayerInteraction : BasePlayerInteraction
{
    public static OutsidePlayerInteraction Instance { get; private set; }

    [Header("References")]
    [SerializeField] private OutsidePickUpContoller outsidePickUpController;
    [SerializeField] private Carabiner carabiner;

    protected override void Awake()
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
        base.SetInteractionEnabled(enabled);
    }

    protected override void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (carabiner != null && carabiner.IsAttached && currentInteractable == carabiner.GetComponent<Interactable>())
            {
                carabiner.TryDetach();
                outsidePickUpController?.TryPickUp(carabiner.GetComponent<Interactable>());
                return;
            }

            if (outsidePickUpController != null)
            {
                if (playerCamera == null) return;

                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerReach))
                {
                    ShelfHangZone shelf = hit.collider.GetComponentInParent<ShelfHangZone>();
                    if (shelf != null)
                    {
                        Transform chosenHangPoint = shelf.GetClosestHangPoint(hit.point);
                        if (chosenHangPoint == null)
                        {
                            Debug.LogWarning("[OutsidePlayerInteraction] No hang point found on shelf.");
                            return;
                        }

                        outsidePickUpController.ForceRelease();
                        carabiner.AttachToRail(shelf, chosenHangPoint);
                        return;
                    }
                }
            }

            if (currentInteractable != null && currentInteractable.IsPickable)
            {
                HandlePickup();
                return;
            }

            if (currentInteractable != null && !currentInteractable.IsPickable)
            {
                currentInteractable.Interact();
            }
        }

        if (Input.GetMouseButtonDown(1) && outsidePickUpController != null && outsidePickUpController.IsHoldingSomething())
        {
            outsidePickUpController.Drop();
        }
    }

    private void HandlePickup()
    {
        if (outsidePickUpController == null) return;
        if (outsidePickUpController.IsHoldingSomething()) outsidePickUpController.Drop();
        outsidePickUpController.TryPickUp(currentInteractable);
    }

    protected override Interactable DetectInteractable()
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
}
