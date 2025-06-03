using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject resumeButton;

    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        isPaused = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}