using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject creditsMenuUI;
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private Image gameTitle;
    [SerializeField] private Button resumeButton;
    [SerializeField] private TMP_Text playButton;
    [SerializeField] private TMP_Text gameText;

    [SerializeField] private TMP_Text sensitivityNumber;
    [SerializeField] private Slider sensitivitySlider;

    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.PauseGame();

        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
        cameraRotation.SetCursorLock(false);

        cameraRotation.enabled = false; // Disable camera rotation at the start

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            gameManager.ResumeGame();
            mainMenuUI.SetActive(false);
            isPaused = false;
            cameraRotation.SetCursorLock(true);
            cameraRotation.enabled = true; // Enable camera rotation when resuming
        }
        else
        {
            gameManager.PauseGame();
            mainMenuUI.SetActive(true);
            isPaused = true;
            cameraRotation.SetCursorLock(false);
            cameraRotation.enabled = false; // Disable camera rotation when paused
        }
    }

    public void StartGame()
    {
        mainMenuUI.SetActive(false);
        gameManager.ResumeGame();
        isPaused = false;
        cameraRotation.SetCursorLock(true);
        cameraRotation.enabled = true; // Enable camera rotation when starting the game

    }
    public void OpenSettings()
    {
        // Logic to open settings menu
        Debug.Log("Settings Opened");
        // Here you would typically enable a settings UI panel or load a settings scene
    }
    public void CloseSettings()
    {
        // Logic to close settings menu
        Debug.Log("Settings Closed");
        // Here you would typically disable a settings UI panel or return to the main menu
    }
    public void ShowCredits()
    {
        // Logic to show credits
        Debug.Log("Credits Shown");
        // Here you would typically enable a credits UI panel or load a credits scene
    }
    public void HideCredits()
    {
        // Logic to hide credits
        Debug.Log("Credits Hidden");
        // Here you would typically disable a credits UI panel or return to the main menu
    }
}
