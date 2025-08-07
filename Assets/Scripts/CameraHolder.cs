using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        if (cameraPosition != null)
            transform.position = cameraPosition.position;
    }
}
