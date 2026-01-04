using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : BasePickUpController
{
    public override void SetAnimator(Animator newAnimator)
    {
        base.SetAnimator(newAnimator);
    }

    public override void SetHoldPoint(Transform newHoldPoint)
    {
        base.SetHoldPoint(newHoldPoint);
    }

    public void TryPickup(Interactable interactable)
    {
        if (animator != null && holdLayerIndex >= 0)
        {
            animator.SetLayerWeight(holdLayerIndex, 1f);
        }

        if (heldObject != null || !interactable.IsPickable)
        { 
            return;
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

        animator.SetLayerWeight(holdLayerIndex, 1f);
    }
}

