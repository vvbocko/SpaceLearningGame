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

    [SerializeField] private TMP_Text sensitivityNumber;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 50f;
    [SerializeField] private float maxSensitivity = 150f;

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

        sensitivitySlider.value = cameraRotation.sensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        SetSensitivity(sensitivitySlider.value);

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

            settingsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
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
    //Settings Menu -----
    public void OpenSettings()
    {
        settingsMenuUI.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
    }
    public void SetSensitivity(float value)
    {
        float scaledValue = Mathf.Lerp(minSensitivity, maxSensitivity, value);
        cameraRotation.sensitivity = scaledValue;
        sensitivityNumber.text = scaledValue.ToString("F1");
    }

    //Credits Menu -----
    public void ShowCredits()
    {
        creditsMenuUI.SetActive(true);
    }
    public void HideCredits()
    {
        creditsMenuUI.SetActive(false);
    }
}
