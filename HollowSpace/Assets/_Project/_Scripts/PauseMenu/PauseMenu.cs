using System;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [Space, SerializeField] private MonoBehaviour[] scriptsToPause;

    private bool isPaused;
    public bool IsPaused => isPaused;

    public UnityEvent OnGamePaused;
    public UnityEvent OnGameUnpaused;

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    public void PauseNoUI()
    {
        isPaused = true;
        Time.timeScale = 0f;
        foreach (var script in scriptsToPause)
        {
            script.enabled = false;
        }
        
        OnGamePaused.Invoke();
    }

    public void UnpauseNoUI()
    {
        isPaused = false;
        Time.timeScale = 1f;
        foreach (var script in scriptsToPause)
        {
            script.enabled = true;
        }
        
        OnGameUnpaused.Invoke();
    }

    public void Pause()
    {
        PauseNoUI();
        pauseMenuUI.SetActive(true);
    }
    public void Unpause()
    {
        UnpauseNoUI();
        pauseMenuUI.SetActive(false);
    }

    public void TogglePauseNoUI()
    {
        if (isPaused) UnpauseNoUI();
        else PauseNoUI();
    }

    public void TogglePause()
    {
        if (isPaused) Unpause();
        else Pause();
    }

    public void MainMenu()
    {
        // TODO: find scene manager and load main menu
    }
    
    public void Quit()
    {
        // TODO: find game manager and quit game
    }
}