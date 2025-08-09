using System.Collections;
using UnityEngine;

public class Carabiner : MonoBehaviour
{
    [SerializeField] private float waitFixTime = 0.3f;
    public bool IsAttached { get; private set; } = false;
    public ShelfHangZone AttachedShelf { get; private set; }
    public Transform AttachedHangPoint { get; private set; }

    private OutsidePickUpContoller pickupController;
    private RailMovement playerRailMovement;

    private void Start()
    {
        pickupController = FindObjectOfType<OutsidePickUpContoller>();
        playerRailMovement = FindObjectOfType<RailMovement>();
    }

    public void AttachToRail(ShelfHangZone shelf, Transform hangPoint)
    {
        if (IsAttached || pickupController == null)
            return;

        if (pickupController.IsHoldingSomething() && pickupController.GetComponentInChildren<Carabiner>() == this)
        {
            pickupController.ForceRelease();

            IsAttached = true;
            AttachedShelf = shelf;
            AttachedHangPoint = hangPoint;

            transform.SetParent(hangPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (pickupController.GetComponent<Animator>() != null)
            {
                int holdLayerIndex = pickupController.GetComponent<Animator>().GetLayerIndex("Hold Layer");
                if (holdLayerIndex >= 0)
                    pickupController.GetComponent<Animator>().SetLayerWeight(holdLayerIndex, 0f);
            }

            if (playerRailMovement != null)
            {
                playerRailMovement.enabled = true;
                shelf.AttachPlayer(playerRailMovement.gameObject);
            }

            // Start the coroutine to verify position after 0.5 seconds
            StartCoroutine(CheckAndFixPosition());
        }
    }

    private IEnumerator CheckAndFixPosition()
    {
        yield return new WaitForSeconds(waitFixTime);

        if (transform.parent != null && transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
            // Optional: reset rotation if needed
            transform.localRotation = Quaternion.Euler(-90f, -90f, 0f);
        }
    }

    // Odpiêcie karabiñczyka od porêczy
    public void DetachFromRail()
    {
        if (!IsAttached)
            return;

        IsAttached = false;
        AttachedShelf = null;
        AttachedHangPoint = null;

        // Od³¹cz karabiñczyk od porêczy
        transform.SetParent(null);

        // Przygotuj Rigidbody do podnoszenia
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // pozwól na podnoszenie i interakcjê
            rb.detectCollisions = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Dezaktywuj ruch po porêczy
        if (playerRailMovement != null)
        {
            playerRailMovement.enabled = false;
        }
    }

    // Wywo³ywane po klikniêciu na porêcz, gdy trzymamy karabiñczyk
    public void TryAttachToClickedRail(ShelfHangZone shelf)
    {
        if (pickupController != null && pickupController.IsHoldingSomething())
        {
            Transform closest = shelf.GetClosestHangPoint(transform.position);
            AttachToRail(shelf, closest);
        }
    }

    // Wywo³ywane po klikniêciu na karabiñczyk, gdy jest przypiêty
    public void TryDetach()
    {
        if (IsAttached)
        {
            DetachFromRail();
        }
    }
}
