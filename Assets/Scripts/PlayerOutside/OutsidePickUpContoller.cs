using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsidePickUpContoller : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwStrength = 1.02f;
    private Interactable heldObject;
    private Rigidbody heldRigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private int holdLayerIndex = 1;

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
        if (animator != null)
            holdLayerIndex = animator.GetLayerIndex("Hold Layer");
    }

    public void SetHoldPoint(Transform newHoldPoint)
    {
        holdPoint = newHoldPoint;
        if (heldObject != null)
        {
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }


    // Pick up any interactable object.
    public void TryPickUp(Interactable interactable)
    {
        if (heldObject != null || !interactable.IsPickable)
            return;

        // If the object is currently attached to a rail, detach it first
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
    }

    /// <summary>
    /// Place the currently held object onto a rail hang point.
    /// </summary>
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

        // IMPORTANT: clear heldObject so hand-follow stops
        heldObject = null;

        if (animator != null && holdLayerIndex >= 0)
            animator.SetLayerWeight(holdLayerIndex, 0f);
    }

    /// <summary>
    /// Drops the held object normally (not onto a rail).
    /// </summary>
    public void Drop()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = false;
            heldRigidbody.detectCollisions = true;
            heldRigidbody.velocity = Vector3.zero;

            heldRigidbody.AddForce(Camera.main.transform.forward * throwStrength, ForceMode.Impulse);
            Vector3 randomTorque = new Vector3(
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f)
            ) * throwStrength;
            heldRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

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
            // Don't detach here — caller will decide new parent
            heldObject = null;
        }
    }

    public bool IsHoldingSomething() => heldObject != null;

    public Interactable GetHeldObject()
    {
        return heldObject;
    }
}
