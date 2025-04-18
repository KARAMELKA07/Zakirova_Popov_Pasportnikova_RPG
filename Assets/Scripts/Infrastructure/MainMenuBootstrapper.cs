using UnityEngine;

public class MainMenuBootstrapper : MonoBehaviour
{
    public MainMenuView mainMenuView;
    public SettingsMenuView settingsMenuView;

    void Start()
    {
        GameBootstrapper.Instance.AudioService.PlayMusic("music");
        new MainMenuController(mainMenuView, settingsMenuView);
    }
}
