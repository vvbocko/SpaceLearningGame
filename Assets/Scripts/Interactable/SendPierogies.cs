using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPierogies : MonoBehaviour
{
    [SerializeField] private Rigidbody[] pierogies; // assign in Inspector
    [SerializeField] private float pushForce = 0.2f;
    [SerializeField] private float rotateForce = 0.1f;
    [SerializeField] private GameObject player;
    [SerializeField] private BoxCollider pierogiCollider;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing entering is the player
        if (other.gameObject == player)
            Debug.Log("Player entered the trigger area");
        {
            foreach (Rigidbody pierog in pierogies)
            {
                if (pierog != null)
                {
                    Vector3 direction = (transform.position - pierog.position).normalized;
                    pierog.AddForce(direction * pushForce, ForceMode.Impulse);
                    Vector3 randomTorque = new Vector3(
                        Random.Range(-rotateForce, rotateForce),
                        Random.Range(-rotateForce, rotateForce),
                        Random.Range(-rotateForce, rotateForce)
                    ) * pushForce;

                    pierog.AddTorque(randomTorque, ForceMode.Impulse);

                    pierogiCollider.enabled = false; // Disable this script after sending the pierogies
                }
            }
        }
    }
}
