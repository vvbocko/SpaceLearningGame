using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMovement : MonoBehaviour
{
    // Add near the other fields (top of class)
    public ShelfHangZone CurrentShelf => currentShelf;
    public int CurrentIndex => currentIndex;


    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float moveCooldown = 1.0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private ShelfHangZone currentShelf;
    private int currentIndex;
    private Transform targetPoint;

    private bool isMovingBetweenPoints = false;
    private float moveCooldownTimer = 0f;

    private Vector3 startPosition;
    private float moveProgress = 0f;

    private void Update()
    {
        // Cooldown countdown
        if (moveCooldownTimer > 0f)
            moveCooldownTimer -= Time.deltaTime;

        // Only accept input if not moving and cooldown has passed
        if (!isMovingBetweenPoints && moveCooldownTimer <= 0f)
        {
            HandleInput();
        }
    }

    private void FixedUpdate()
    {
        if (targetPoint != null && isMovingBetweenPoints)
        {
            moveProgress += Time.fixedDeltaTime / moveDuration;
            moveProgress = Mathf.Clamp01(moveProgress);

            float curveValue = movementCurve.Evaluate(moveProgress);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPoint.position, curveValue);

            // movementDelta must be calculated before changing position
            float movementDelta = newPosition.x - transform.position.x;

            transform.position = newPosition;

            // Animation triggers
            if (moveProgress < 1f)
            {
                if (movementDelta > 0.01f)
                {
                    animator.SetTrigger("MoveRight");
                    animator.SetBool("IsIdle", false);
                }
                else if (movementDelta < -0.01f)
                {
                    animator.SetTrigger("MoveLeft");
                    animator.SetBool("IsIdle", false);
                }
            }
            else
            {
                // Finished moving
                isMovingBetweenPoints = false;
                moveCooldownTimer = moveCooldown;
                moveProgress = 0f;
                transform.position = targetPoint.position; // final snap
                animator.SetBool("IsIdle", true);
            }
        }
    }

    public void MoveToPoint(Transform newTarget)
    {
        if (!isMovingBetweenPoints && moveCooldownTimer <= 0f)
        {
            targetPoint = newTarget;
            startPosition = transform.position;
            moveProgress = 0f;
            isMovingBetweenPoints = true;
        }
    }

    void HandleInput()
    {
        if (currentShelf == null) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(1);
        }
    }

    void TryMove(int direction)
    {
        if (currentShelf.TryGetNextPoint(currentIndex, direction, out Transform nextPoint))
        {
            currentIndex += direction;
            MoveToPoint(nextPoint);
        }
    }

    public void SnapToShelf(ShelfHangZone shelf, Transform hangPoint)
    {
        currentShelf = shelf;
        currentIndex = shelf.GetIndexOfPoint(hangPoint);

        if (!isMovingBetweenPoints)
        {
            // Smooth transition to the new rail
            targetPoint = hangPoint;
            startPosition = transform.position;
            moveProgress = 0f;
            isMovingBetweenPoints = true;
        }
        else
        {
            // In case it's called while still moving, just snap (fallback)
            transform.position = hangPoint.position;
        }
    }
}
