using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused;

    [SerializeField] UnityEvent PausingTheGame;
    [SerializeField] UnityEvent ResumingTheGame;
    void Update()
    {
        
    }

    public void PauseMenuButtonPress()
    {
        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        gameIsPaused = false;
        ResumingTheGame.Invoke();
    }
    void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
        gameIsPaused = true;
        PausingTheGame.Invoke();
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerStatus.hasSword = false;
        PlayerStatus.hasMeleeUlt = false;
        PlayerStatus.hasRangedUlt = false;
        Resume();
    }

    public void QuitGame()
    {
        Debug.Log("Quiting");
        Application.Quit();
    }
}
