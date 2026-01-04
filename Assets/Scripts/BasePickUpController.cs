using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePickUpController : MonoBehaviour
{
    [SerializeField] protected Transform holdPoint;
    [SerializeField] protected float throwStrength = 1.02f;
    protected Interactable heldObject;
    protected Rigidbody heldRigidbody;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int holdLayerIndex = 1;

    public virtual void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
        if (animator != null)
        {
            holdLayerIndex = animator.GetLayerIndex("Hold Layer");
        }
    }

    public virtual void SetHoldPoint(Transform newHoldPoint)
    {
        holdPoint = newHoldPoint;
        if (heldObject != null)
        {
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    public virtual void Drop()
    {
        if (animator != null && holdLayerIndex >= 0)
        {
            animator.SetLayerWeight(holdLayerIndex, 0f);
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
        if (animator != null && holdLayerIndex >= 0)
        {
            animator.SetLayerWeight(holdLayerIndex, 0f);
        }
    }

    public virtual bool IsHoldingSomething() => heldObject != null;
}
