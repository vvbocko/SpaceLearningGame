using UnityEngine;

public class PlayerInteraction : BasePlayerInteraction
{
    public static PlayerInteraction Instance { get; private set; }

    [SerializeField] private PickUpController pickupController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public override void SetInteractionEnabled(bool enabled)
    {
        base.SetInteractionEnabled(enabled);
        GetComponent<ZeroGravityMovement>().enabled = enabled;
    }

    protected override void HandleInput()
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

    private void HandlePickup()
    {
        if (pickupController.IsHoldingSomething())
        {
            pickupController.Drop();
        }
        pickupController.TryPickup(currentInteractable);
    }

    protected override Interactable DetectInteractable()
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
}