using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Carabiner : MonoBehaviour
{
    [Header("Pinning / movement")]
    [SerializeField, Tooltip("Time to lerp the carabiner between hang points")]
    private float moveDuration = 0.4f;

    [SerializeField, Tooltip("Curve used for carabiner movement")]
    private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField, Tooltip("Safety fix delay to re-snap local transform after parent change")]
    private float waitFixTime = 0.25f;

    // state
    public bool IsAttached { get; private set; } = false;
    public ShelfHangZone AttachedShelf { get; private set; }
    public Transform AttachedHangPoint { get; private set; }
    private int attachedIndex = -1;

    // runtime refs
    private OutsidePickUpContoller outsidePickUpController;
    private RailMovement playerRailMovement;
    private Rigidbody rb;

    // internal
    private bool isMovingBetweenHangPoints = false;
    private Coroutine moveCoroutine;

    // --- scale stability ---
    // Desired visual world scale that we will preserve across parenting/unparenting:
    private Vector3 desiredWorldScale;

    void Start()
    {
        outsidePickUpController = FindObjectOfType<OutsidePickUpContoller>();
        playerRailMovement = FindObjectOfType<RailMovement>();
        rb = GetComponent<Rigidbody>();

        // Store the desired world scale at start (this is what we want to preserve)
        desiredWorldScale = transform.lossyScale;
    }

    // Helper to compute the localScale we must set for a given parent so that the resulting world scale equals desiredWorldScale
    private Vector3 ComputeLocalScaleForParent(Transform parent)
    {
        Vector3 parentScale = parent != null ? parent.lossyScale : Vector3.one;

        // avoid division by zero
        float sx = parentScale.x == 0f ? 1f : parentScale.x;
        float sy = parentScale.y == 0f ? 1f : parentScale.y;
        float sz = parentScale.z == 0f ? 1f : parentScale.z;

        return new Vector3(
            desiredWorldScale.x / sx,
            desiredWorldScale.y / sy,
            desiredWorldScale.z / sz
        );
    }

    /// <summary>
    /// Attach the carabiner to a specific hang point on a shelf (caller should ForceRelease() first).
    /// </summary>
    public void AttachToRail(ShelfHangZone shelf, Transform hangPoint)
    {
        if (IsAttached) return;
        if (shelf == null || hangPoint == null) return;

        // Set state
        IsAttached = true;
        AttachedShelf = shelf;
        AttachedHangPoint = hangPoint;
        attachedIndex = shelf.GetIndexOfPoint(hangPoint);

        // Parent & zero local transform immediately (keep local values after parenting)
        transform.SetParent(hangPoint, false);

        // Ensure visual scale stays the same in world space
        transform.localScale = ComputeLocalScaleForParent(hangPoint);

        // Snap local transform
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);

        // Freeze physics while pinned
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Ensure player RailMovement reference
        if (playerRailMovement == null)
            playerRailMovement = FindObjectOfType<RailMovement>();

        if (playerRailMovement != null)
        {
            playerRailMovement.enabled = true;
            shelf.AttachPlayer(playerRailMovement.gameObject);
        }

        // Safety snap after a short delay to fix any transform mismatch
        StartCoroutine(CheckAndFixPosition());
    }

    /// <summary>
    /// Detach the carabiner from the rail so it becomes a physical pickupable object.
    /// </summary>
    public void DetachFromRail()
    {
        if (!IsAttached) return;

        IsAttached = false;
        AttachedShelf = null;
        AttachedHangPoint = null;
        attachedIndex = -1;

        // Unparent so pickup logic can re-parent it to the player hold point
        transform.SetParent(null, false);

        // After unparenting, restore localScale so world scale equals desired world scale
        transform.localScale = desiredWorldScale;

        // Re-enable physics so it can be picked up
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Optionally disable player rail movement if you want that behavior
        if (playerRailMovement != null)
        {
            playerRailMovement.enabled = false;
        }
    }

    private IEnumerator CheckAndFixPosition()
    {
        yield return new WaitForSeconds(waitFixTime);

        if (transform.parent != null)
        {
            // Ensure correct local position/rotation & scale (safety)
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);
            transform.localScale = ComputeLocalScaleForParent(transform.parent);
        }
    }

    // Called by OutsidePlayerInteraction when the player clicks a rail while holding the carabiner
    public void TryAttachToClickedRail(ShelfHangZone shelf)
    {
        if (outsidePickUpController == null)
            outsidePickUpController = FindObjectOfType<OutsidePickUpContoller>();

        if (outsidePickUpController != null && outsidePickUpController.IsHoldingSomething())
        {
            Transform closest = shelf.GetClosestHangPoint(transform.position);
            AttachToRail(shelf, closest);
        }
    }

    // Called by OutsidePlayerInteraction when clicking an attached carabiner to pick it up
    public void TryDetach()
    {
        if (IsAttached)
            DetachFromRail();
    }

    private void Update()
    {
        // Only run follower logic while attached and shelf/player are valid
        if (!IsAttached || AttachedShelf == null || playerRailMovement == null || playerRailMovement.CurrentShelf == null)
            return;

        // Only operate when both are on same shelf
        if (playerRailMovement.CurrentShelf != AttachedShelf)
            return;

        // Player index and attached index
        int playerIndex = playerRailMovement.CurrentIndex;
        int delta = playerIndex - attachedIndex;

        // If the player moved more than 1 hangpoint away, move the carabiner one step towards player.
        if (Mathf.Abs(delta) > 1 && !isMovingBetweenHangPoints)
        {
            int direction = (int)Mathf.Sign(delta); // +1 if player is ahead, -1 if behind
            int targetIndex = attachedIndex + direction; // move one step toward player

            // safety check bounds
            if (targetIndex >= 0 && targetIndex < AttachedShelf.hangPoints.Length)
            {
                Transform targetPoint = AttachedShelf.hangPoints[targetIndex];
                // start smooth movement coroutine
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToHangPointCoroutine(targetPoint, targetIndex));
            }
        }
    }

    private IEnumerator MoveToHangPointCoroutine(Transform targetPoint, int targetIndex)
    {
        if (targetPoint == null) yield break;

        isMovingBetweenHangPoints = true;

        // Capture start and end world transforms
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetPoint.position;
        // rotation we want relative to hangPoint
        Quaternion endRot = targetPoint.rotation * Quaternion.Euler(-90f, -90f, 0f);

        // Unparent during motion to avoid local-space overrides
        transform.SetParent(null, false);

        // After unparenting, ensure world-size stays same
        transform.localScale = desiredWorldScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, moveDuration);
            float eval = moveCurve.Evaluate(Mathf.Clamp01(t));
            transform.position = Vector3.Lerp(startPos, endPos, eval);
            transform.rotation = Quaternion.Slerp(startRot, endRot, eval);
            yield return null;
        }

        // Final snap & parent
        transform.position = endPos;
        transform.rotation = endRot;

        // Parent and compute localScale so world size is preserved
        transform.SetParent(targetPoint, false);
        transform.localScale = ComputeLocalScaleForParent(targetPoint);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);

        // Update tracked index/state
        attachedIndex = targetIndex;
        AttachedHangPoint = targetPoint;

        // ensure rigidbody remains kinematic (pinned)
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // small delay to make sure nothing else overwrites local transform
        StartCoroutine(CheckAndFixPosition());

        isMovingBetweenHangPoints = false;
        moveCoroutine = null;
    }
}
