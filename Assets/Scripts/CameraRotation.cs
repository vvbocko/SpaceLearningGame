using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    [Header("Camera Rotation")]
    
    public float sensitivityX = 100f;
    public float sensitivityY = 100f;

    [SerializeField] private float maxRotation = 80f;

    private float xRotation;
    private float yRotation;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {

        RotationHandler();
    }

    private void RotationHandler()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f , maxRotation);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
