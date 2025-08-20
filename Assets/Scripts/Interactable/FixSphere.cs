using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSphere : MonoBehaviour
{
    [SerializeField] private GameObject fixSphere;
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        fixSphere.transform.rotation = Quaternion.Euler(0, fixSphere.transform.rotation.eulerAngles.y + Time.deltaTime * rotationSpeed, 0);
    }
}
