using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsidePickUpContoller : BasePickUpController
{
    [SerializeField] private GameManager gameManager;
    private bool finishedCarabinerTask = false;
    private bool finishedRailTask = false;

    public override void SetAnimator(Animator newAnimator)
    {
        base.SetAnimator(newAnimator);
    }

    public override void SetHoldPoint(Transform newHoldPoint)
    {
        base.SetHoldPoint(newHoldPoint);
    }

    public void TryPickUp(Interactable interactable)
    {
        if (heldObject != null || !interactable.IsPickable)
            return;

        if (interactable.transform.parent != null && interactable.transform.parent.CompareTag("HangPoint"))
        {
            interactable.transform.SetParent(null);
        }

        heldObject = interactable;
        heldRigidbody = heldObject.GetComponent<Rigidbody>();

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = true;
            heldRigidbody.detectCollisions = false;
        }

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (animator != null && holdLayerIndex >= 0)
            animator.SetLayerWeight(holdLayerIndex, 1f);

        if (interactable.GetComponent<Carabiner>() != null && !finishedCarabinerTask)
        {
            gameManager.CarabinerPicked();
            finishedCarabinerTask = true;
        }
    }

    public void PlaceOnRail(Transform hangPoint)
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(hangPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = true;
            heldRigidbody.detectCollisions = false;
            heldRigidbody = null;
        }

        heldObject = null;
        if (animator != null && holdLayerIndex >= 0)
            animator.SetLayerWeight(holdLayerIndex, 0f);
    }

    public void ForceRelease()
    {
        if (heldObject != null)
        {
            heldObject = null;
        }
    }

    public Interactable GetHeldObject()
    {
        return heldObject;
    }
}
