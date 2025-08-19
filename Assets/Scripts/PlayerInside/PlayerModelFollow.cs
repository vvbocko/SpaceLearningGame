using UnityEngine;

public class PlayerModelFollow : MonoBehaviour
{
    [SerializeField] private Transform orientation; // Assign Orientation object

    private Vector3 localOffset;

    void Start()
    {
        localOffset = transform.localPosition;
    }

    void LateUpdate()
    {
        // Create a rotation that only includes the Y-axis from Orientation
        Quaternion flatYRotation = Quaternion.Euler(0f, orientation.eulerAngles.y, 0f);

        // Apply position offset relative to horizontal rotation only
        transform.position = orientation.position + flatYRotation * localOffset;

        // Rotate model to match horizontal camera direction
        transform.rotation = flatYRotation;
    }
}
