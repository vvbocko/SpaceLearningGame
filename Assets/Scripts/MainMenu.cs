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

    [SerializeField] private GameObject firstPopUp;
    [SerializeField] private GameObject secondPopUp;
    [SerializeField] private GameObject thirdPopUp;
    [SerializeField] private GameObject endingMessage;

    [SerializeField] private GameObject firstInstruction;
    [SerializeField] private GameObject secondInstruction;

    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private Button resumeButton;

    [SerializeField] private TMP_Text sensitivityNumber;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 50f;
    [SerializeField] private float maxSensitivity = 150f;

    public bool isPaused = false;
    public bool wasPopUpShown = false;
    public bool isSecondPopUpShown = false;
    public bool isThirdPopUpShown = false;
    public bool isEndingMessageShown = false;

    void Start()
    {
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);

        firstPopUp.SetActive(false);
        secondPopUp.SetActive(false);
        thirdPopUp.SetActive(false);

        firstInstruction.SetActive(false);
        secondInstruction.SetActive(false);

        cameraRotation.SetCursorLock(false);
        cameraRotation.enabled = false;

        sensitivitySlider.value = cameraRotation.sensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        SetSensitivity(sensitivitySlider.value);

    }
    void Update()
    {
        // ESC only works outside of dialogue
        if (Input.GetKeyDown(KeyCode.Escape) && wasPopUpShown && !isSecondPopUpShown && !isThirdPopUpShown && !isEndingMessageShown &&
            gameManager.CurrentState != GameState.Dialogue)
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
            cameraRotation.enabled = true;

            settingsMenuUI.SetActive(false);
            creditsMenuUI.SetActive(false);
        }
        else
        {
            gameManager.PauseGame();
            mainMenuUI.SetActive(true);
            isPaused = true;
            cameraRotation.SetCursorLock(false);
            cameraRotation.enabled = false;
        }
    }

    public void StartGame()
    {
        if (!wasPopUpShown)
        {
            mainMenuUI.SetActive(false);
            gameManager.BeginMission();
            return;
        }
        gameManager.ResumeGame();
        mainMenuUI.SetActive(false);
        isPaused = false;
        cameraRotation.SetCursorLock(true);
        cameraRotation.enabled = true;
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

    //PopUp Panel -----
    public void ShowPopUp()
    {
        firstPopUp.SetActive(true);
        firstInstruction.SetActive(true);
        gameManager.currentObjectiveIndex = 0;
    }
    public void ExitPopUp()
    {
        firstPopUp.SetActive(false);

        wasPopUpShown = true;
        gameManager.ResumeGame();
        isPaused = false;
        cameraRotation.SetCursorLock(true);
        cameraRotation.enabled = true;
    }

    public void ShowPopUpSecond()
    {
        secondPopUp.SetActive(true);
        firstInstruction.SetActive(false);
        secondInstruction.SetActive(true);
        gameManager.PauseGame();
        isPaused = true;
        isSecondPopUpShown = true;
        cameraRotation.SetCursorLock(false);
        cameraRotation.enabled = false;
    }
    public void ExitPopUpSecond()
    {
        secondPopUp.SetActive(false);

        gameManager.ResumeGame();
        isPaused = false;
        isSecondPopUpShown = false;
        cameraRotation.SetCursorLock(true);
        cameraRotation.enabled = true;
    }

    public void ShowPopUpThird()
    {
        if (isThirdPopUpShown)
        {
            return; 
        }
        thirdPopUp.SetActive(true);

        gameManager.PauseGame();
        isPaused = true;
        isThirdPopUpShown = true;
        cameraRotation.SetCursorLock(false);
        cameraRotation.enabled = false;
        
    }
    public void ExitPopUpThird()
    {
        thirdPopUp.SetActive(false);

        gameManager.ResumeGame();
        isPaused = false;
        isThirdPopUpShown = false;
        cameraRotation.SetCursorLock(true);
        cameraRotation.enabled = true;
    }
    public void ShowEndingMessage()
    {
        endingMessage.SetActive(true);

        isEndingMessageShown = true;
        gameManager.PauseGame();
        isPaused = true;
        cameraRotation.SetCursorLock(false);
        cameraRotation.enabled = false;
    }
    public void ReturnToMainMenu()
    {
        isEndingMessageShown = false;
        gameManager.RestartGame();
    }

}
