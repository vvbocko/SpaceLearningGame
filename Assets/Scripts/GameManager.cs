using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu; // Reference to the main menu UI

    // Start is called before the first frame update
    void Start()
    {
        PauseGame(); // Start the game in a paused state
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Pause the game - public method
    //Resume the game - public method

    public void PauseGame()
    {
        Time.timeScale = 0f; // Freeze the game

    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

}
