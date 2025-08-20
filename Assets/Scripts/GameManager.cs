using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Dialogue
}

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private MainMenu menuManager;
    [SerializeField] private GameObject astronautSuit;
    [SerializeField] private GameObject airLeakPlace;

    [SerializeField] private GameObject objectiveMenu;
    [SerializeField] private GameObject blockage1;
    [SerializeField] private GameObject blockage2;
    [SerializeField] private GameObject airlock1;
    [SerializeField] private GameObject airlock2;

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject carabinerArrow;
    [SerializeField] private GameObject pipeArrow;

    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private AudioSource insideMusic;
    [SerializeField] private AudioSource outsideMusic;

    [Header("Objectives")]
    private string[] objectives =
    {
        "Pogadaj z dwójk¹ astronautów (0/2)",
        "Za³ó¿ skafander astronauty",
        "Opuœæ stacjê kosmiczn¹",
        "Podnieœ karabiñczyk",
        //"Przypnij do porêczy",
        "Napraw wyciek powietrza",
        "Wróæ do œrodka stacji kosmicznej"
    };

    public int currentObjectiveIndex = -1;
    private bool gamePaused = false;
    public bool finishedFixing = false;
    private int astronautsTalkedTo = 0;
    private HashSet<Interactable> talkedAstronauts = new HashSet<Interactable>();
    public GameState CurrentState { get; private set; } = GameState.Paused;

    void Start()
    {
        PauseGame();
        objectiveMenu.SetActive(false);
        blockage1.SetActive(true);
        blockage2.SetActive(true);
        airlock1.SetActive(true);
        airlock2.SetActive(false);
        arrow.SetActive(false);
        carabinerArrow.SetActive(false);
        pipeArrow.SetActive(false);
        astronautSuit.SetActive(true);
        airLeakPlace.SetActive(true);

        PlayInsideMusic();
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case GameState.Dialogue:
                Time.timeScale = 1f; // keep animations running
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }

    public void BeginMission()
    {
        objectiveMenu.SetActive(true);
        menuManager.ShowPopUp();
        UpdateObjectiveText(objectives[currentObjectiveIndex]);
    }

    public void AdvanceObjective()
    {
        currentObjectiveIndex++;
        if (currentObjectiveIndex < objectives.Length)
        {
            UpdateObjectiveText(objectives[currentObjectiveIndex]);
        }
        else
        {
            UpdateObjectiveText("Misja zakoñczona! Dziêkujemy za grê.");
        }
    }

    public void UpdateObjectiveText(string text)
    {
        objectiveText.text = text;
    }

    public void UpdateAstronautProgressFromNPC(Interactable npc)
    {
        if (!talkedAstronauts.Contains(npc))
        {
            talkedAstronauts.Add(npc);
            astronautsTalkedTo++;
            objectiveText.text = $"Pogadaj z dwójk¹ astronautów ({astronautsTalkedTo}/2)";

            if (astronautsTalkedTo >= 2)
            {
                AdvanceObjective(); // goes to "Za³ó¿ skafander astronauty"
                blockage1.SetActive(false);
            }
        }
    }

    public void SuitWorn()
    {
        if (currentObjectiveIndex == 1) // "Za³ó¿ skafander astronauty"
        {
            AdvanceObjective();
            blockage1.SetActive(true);
            blockage2.SetActive(false);
            arrow.SetActive(true);
            
        }
    }

    public void ExitedStation()
    {
        if (currentObjectiveIndex == 2) // "Opuœæ stacjê kosmiczn¹" 
        {
            AdvanceObjective();
            menuManager.ShowPopUpSecond();
            blockage1.SetActive(false);
            arrow.SetActive(false);
            carabinerArrow.SetActive(true);
        }
    }
    public void CarabinerPicked()
    {
        if (currentObjectiveIndex == 3) // "Podnieœ karabiñczyk"
        {
            AdvanceObjective();
            carabinerArrow.SetActive(false);
            pipeArrow.SetActive(true);
        }
    }

    //public void PinnedToRail()
    //{
    //    if (currentObjectiveIndex == 4) // "Przypnij do porêczy"
    //    {
    //        AdvanceObjective();
    //        pipeArrow.SetActive(false);
    //    }
    //}

    public void AirLeakFixed()
    {
        if (currentObjectiveIndex == 4) // "Napraw wyciek powietrza"
        {
            AdvanceObjective();
            menuManager.ShowPopUpThird();
            airlock2.SetActive(true);
            airLeakPlace.SetActive(false);
            finishedFixing = true;
        }
    }

    public void ReturnedToStation()
    {
        if (currentObjectiveIndex == 5) // "Wróæ do œrodka stacji kosmicznej"
        {
            menuManager.ShowEndingMessage();
        }
    }

    public void PlayInsideMusic()
    {
        outsideMusic.Stop();
        if (!insideMusic.isPlaying)
        {
            insideMusic.Play();
        }
    }

    public void PlayOutsideMusic()
    {
        insideMusic.Stop();
        if (!outsideMusic.isPlaying)
        {
            outsideMusic.Play();
        }
    }

    public void PauseGame() => SetState(GameState.Paused);
    public void ResumeGame() => SetState(GameState.Playing);

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
