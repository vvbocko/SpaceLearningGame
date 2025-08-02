using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwStrength = 1.02f;
    private Interactable heldObject;
    private Rigidbody heldRigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private int holdLayerIndex = 1; // odpowiednik: Animator.StringToHash("Hold Layer")

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
        if (animator != null)
        {
            holdLayerIndex = animator.GetLayerIndex("Hold Layer");
        }
    }

    public void SetHoldPoint(Transform newHoldPoint)
    {
        holdPoint = newHoldPoint;

        // Je¿eli coœ trzymamy – przesuñ do nowego HoldPointa
        if (heldObject != null)
        {
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    public void TryPickup(Interactable interactable)
    {
        if (animator != null && holdLayerIndex >= 0)
        {
            animator.SetLayerWeight(holdLayerIndex, 1f); // w TryPickup
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

    public void Drop()
    {
        if (animator != null && holdLayerIndex >= 0)
        {
            animator.SetLayerWeight(holdLayerIndex, 0f); // w Drop
        }

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

        animator.SetLayerWeight(holdLayerIndex, 0f);
    }



    public bool IsHoldingSomething() => heldObject != null;
}

