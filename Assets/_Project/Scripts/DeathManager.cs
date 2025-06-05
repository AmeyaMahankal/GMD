using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject firstSelectedButton;

    private bool hasDied = false;

    public void ShowGameOver()
    {
        if (hasDied) return;

        hasDied = true;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Replace with your actual main menu scene name
    }
}
