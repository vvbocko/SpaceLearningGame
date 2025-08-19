using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixingMiniGame : MonoBehaviour
{
    [SerializeField] private CameraRotation playerRotation;
    [SerializeField] private ZeroGravityMovement playerMovement;

    [SerializeField] private GameObject fishingMiniGameUI;
    [SerializeField] private GameObject interactableBox;
    [SerializeField] private Scrollbar playerBar;
    [SerializeField] private Scrollbar fishBar;
    [SerializeField] private Slider progressBar;

    [SerializeField] private RectTransform playerHandle;
    [SerializeField] private RectTransform fishHandle;

    [Header("Player Bar")]
    [SerializeField] private float gravity = 15f;
    [SerializeField] private float liftForce = 30f;
    [SerializeField] private float maxSpeed = 5f;
    private float velocity = 0f;
    [Header("Fish Bar")]
    [SerializeField] private float fishSpeed = 0.5f;
    [SerializeField] private float maxSpeedChange = 1f;
    [SerializeField] private float decisionRate = 1.5f;
    private float targetVelocity = 0f;
    [Header("Progress Bar")]
    [SerializeField] private float progressSpeed = 2f;

    void Start()
    {
        interactableBox.GetComponent<Interactable>().enabled = true;
        DisableUI();
        //set canvas component active = false; -- aditional method for start and end handle mechanic 
        playerHandle = playerBar.handleRect;
        fishHandle = fishBar.handleRect;

        StartCoroutine(FishAI());
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            velocity += Time.deltaTime * liftForce;

        }
        else
        {
            velocity -= Time.deltaTime * gravity;
        }
        HandleRandomBar();
        HandlePlayerBar();
        HandleProgressBar();

    }

    private void HandlePlayerBar()
    {
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);

        playerBar.value += velocity * Time.deltaTime;

        if (playerBar.value <= 0f || playerBar.value >= 1f)
        {
            playerBar.value = Mathf.Clamp01(playerBar.value);
            velocity = 0f;
        }
    }
    private void HandleRandomBar()
    {
        fishBar.value += targetVelocity * Time.deltaTime;

        if (fishBar.value <= 0f || fishBar.value >= 1f)
        {
            fishBar.value = Mathf.Clamp01(fishBar.value);
            targetVelocity = -targetVelocity * 0.5f; // bounce back a bit
        }
    }
    IEnumerator FishAI()
    {
        while (true)
        {
            // Wait a bit before changing behavior
            yield return new WaitForSeconds(Random.Range(0.5f, decisionRate));

            // Pick a new random velocity
            targetVelocity = Random.Range(-maxSpeedChange, maxSpeedChange);
        }
    }
    private void HandleProgressBar()
    {
        if(progressBar.value >= 1f)
        {
            DisableUI();
        }
        if (IsOverlapping())
        {
            progressBar.value += Time.deltaTime * progressSpeed;
        }
        else
        {
            progressBar.value -= Time.deltaTime * progressSpeed;
        }
        progressBar.value = Mathf.Clamp01(progressBar.value);
    }
    private bool IsOverlapping()
    {
        Rect playerRect = GetScreenRect(playerHandle);
        Rect fishRect = GetScreenRect(fishHandle);

        return playerRect.Overlaps(fishRect);
    }
    private Rect GetScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private void DisableUI()
    {
        progressBar.value = 0;

        fishingMiniGameUI.SetActive(false);
        playerBar.enabled = false;
        fishBar.enabled = false;
        progressBar.enabled = false;

        playerRotation.enabled = true;
        playerMovement.enabled = true;
    }
    public void EnableUI()
    {
        fishingMiniGameUI.SetActive(true);
        playerBar.enabled = true;
        fishBar.enabled = true;
        progressBar.enabled = true;

        playerRotation.enabled = false;
        playerMovement.enabled = false;
    }

    private void MakeUninteractable()
    {
        interactableBox.GetComponent<Interactable>().enabled = false;
    }

}
