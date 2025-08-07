using UnityEngine;

public class ShelfHangZone : MonoBehaviour
{
    public Transform[] hangPoints;

    public Transform GetClosestHangPoint(Vector3 fromPosition)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var point in hangPoints)
        {
            float dist = Vector3.Distance(fromPosition, point.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = point;
            }
        }

        return closest;
    }

    public int GetIndexOfPoint(Transform point)
    {
        for (int i = 0; i < hangPoints.Length; i++)
        {
            if (hangPoints[i] == point)
                return i;
        }
        return -1;
    }

    public bool TryGetNextPoint(int currentIndex, int direction, out Transform result)
    {
        int target = currentIndex + direction;
        if (target >= 0 && target < hangPoints.Length)
        {
            result = hangPoints[target];
            return true;
        }

        result = null;
        return false;
    }

    public void AttachPlayer(GameObject player)
    {
        RailMovement hanging = player.GetComponent<RailMovement>();
        if (hanging != null)
        {
            Transform closest = GetClosestHangPoint(player.transform.position);
            hanging.SnapToShelf(this, closest);
        }
    }
}