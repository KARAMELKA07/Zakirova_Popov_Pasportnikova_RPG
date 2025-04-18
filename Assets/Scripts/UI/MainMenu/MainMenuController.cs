using UnityEngine;

public class MainMenuController
{
    private MainMenuView view;

    public MainMenuController(MainMenuView view)
    {
        this.view = view;

        view.playButton.onClick.AddListener(OnPlayClicked);
        view.settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnPlayClicked()
    {
        GameBootstrapper.Instance.GameStateService.LoadScene("Gameplay");
    }

    private void OnSettingsClicked()
    {
        // позже откроем настройки
        Debug.Log("Настройки пока не реализованы");
    }
}