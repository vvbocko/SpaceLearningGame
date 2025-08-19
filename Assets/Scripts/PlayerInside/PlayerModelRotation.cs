using UnityEngine;

public class PlayerModelRotation : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerModelMale;
    [SerializeField] private Transform playerModelSuit;

    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        // Get the forward direction on the horizontal plane
        Vector3 lookDir = orientation.forward;
        lookDir.y = 0;

        // Don't rotate if the direction is zero
        if (lookDir == Vector3.zero) return;

        // Smoothly rotate the player model to face that direction
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        playerModelMale.rotation = Quaternion.Slerp(playerModelMale.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        playerModelSuit.rotation = Quaternion.Slerp(playerModelSuit.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

