using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwStrength = 1.02f;
    private Interactable heldObject;
    private Rigidbody heldRigidbody;

    public void TryPickup(Interactable interactable)
    {
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
    }

    public void Drop()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);

        if (heldRigidbody != null)
        {
            heldRigidbody.isKinematic = false;
            heldRigidbody.detectCollisions = true;

            // Optional: give a little drop force
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
    }


    public bool IsHoldingSomething() => heldObject != null;
}

