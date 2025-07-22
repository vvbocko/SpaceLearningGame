using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PickupController pickupController;
    [SerializeField] private Camera playerCamera;
    private Interactable currentInteractable;

    [SerializeField] private float playerReach = 3f;
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

        if (Input.GetMouseButtonDown(1) && pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
            return;
        }

        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            if (currentInteractable.IsPickable)
            {
                if (pickupController.IsHoldingSomething())
                {
                    pickupController.Drop(); // Optional: drop current to pick up another
                }

                pickupController.TryPickup(currentInteractable);
            }
            else
            {
                currentInteractable.Interact();
            }
        }
    }


    Interactable DetectInteractable()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, playerReach))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable) && interactable.enabled)
            {
                return interactable;
            }
        }

        return null;
    }

    void UpdateCurrentInteractable(Interactable newInteractable)
    {
        if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
        }

        currentInteractable = newInteractable;

        if (currentInteractable != null)
        {
            currentInteractable.EnableOutline();
        }
    }
}
