using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject leaderboardsMenu;
    
    private GameObject currentMenu;
    


    public void Start() {
        currentMenu = mainMenu;
        ShowMenu(mainMenu);
    }
    
    public void ShowMenu(GameObject menuToShow)
    {
        // Hide the current menu
        if (currentMenu != null)
            currentMenu.SetActive(false);

        // Show the new menu
        menuToShow.SetActive(true);
        currentMenu = menuToShow;
        
        if (currentMenu != mainMenu) {
         SelectButton(currentMenu, "ReturnToMainMenuButton");
        }
        else {
            SelectButton(currentMenu, "StartButton");
        }
    }
    public void StartGame() {
        SceneManager.LoadScene("Master");
        Debug.Log("Start");
    }
    
    private void SelectButton(GameObject menu, string buttonId) {
        // Find the Back button inside the active menu
        GameObject backButton = menu.transform.Find(buttonId)?.gameObject;

        if (backButton != null) {
            EventSystem.current.SetSelectedGameObject(backButton);
        }
    }
    
    public void ReturnToMainMenu() {
        ShowMenu(mainMenu);
    }

    public void ExitGame() {
        Application.Quit();
        Debug.Log("Exit Game");
    }

    public void OpenSettings() {
        ShowMenu(settingsMenu);
        Debug.Log("Open Settings");
    }
    
    public void OpenLeaderboard() {
        ShowMenu(leaderboardsMenu);
        Debug.Log("Open Scoreboard");
    }
    
    

}