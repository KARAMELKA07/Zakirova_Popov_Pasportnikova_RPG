using UnityEngine;

public class MainMenuController
{
    private MainMenuView view;

    private SettingsMenuController settingsController;
    private SettingsMenuView settingsView;
    private GameObject mainMenuRoot;

    public MainMenuController(MainMenuView view, SettingsMenuView settingsView)
    {
        this.view = view;
        this.settingsView = settingsView;
        this.mainMenuRoot = view.mainMenuRoot;

        view.playButton.onClick.AddListener(OnPlayClicked);
        view.settingsButton.onClick.AddListener(OnSettingsClicked);

        settingsController = new SettingsMenuController(settingsView);
        settingsController.OnBack = ShowMainMenu;
    }

    private void OnSettingsClicked()
    {
        mainMenuRoot.SetActive(false);
        settingsController.Show();
    }

    private void ShowMainMenu()
    {
        mainMenuRoot.SetActive(true);
    }



    private void OnPlayClicked()
    {
        Debug.Log("lol");
        GameBootstrapper.Instance.GameStateService.LoadScene("Gameplay");
    }
    
}