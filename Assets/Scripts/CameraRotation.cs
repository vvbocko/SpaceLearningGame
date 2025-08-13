using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject outsidePlayer;

    [Header("Camera Rotation")]
    
    public float sensitivity = 100f;

    [SerializeField] public float maxRotationX = 74f;
    [SerializeField] private float maxRotationY = 80f;

    private float xRotation;
    private float yRotation;

    // Public method to control cursor from other scripts
    public void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
        
        // Optional: Pause rotation when cursor is unlocked
        if (!locked) 
        {
            xRotation = transform.localEulerAngles.x;
            yRotation = transform.localEulerAngles.y;
        }
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            RotationHandler();
        }
    }
    private void RotationHandler()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxRotationX, maxRotationX);

        yRotation += mouseX;

        if (outsidePlayer.activeInHierarchy)
        {
            float playerYaw = outsidePlayer.transform.eulerAngles.y;

            // Get shortest angle difference between yRotation and playerYaw (-180..180)
            float relativeYaw = Mathf.DeltaAngle(playerYaw, yRotation);

            // Clamp relativeYaw to limits
            relativeYaw = Mathf.Clamp(relativeYaw, -maxRotationY, maxRotationY);

            // Reconstruct yRotation as playerYaw + clamped relativeYaw
            yRotation = playerYaw + relativeYaw;
        }

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        if (orientation != null)
            orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private float WrapAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
