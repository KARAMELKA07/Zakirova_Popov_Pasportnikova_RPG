using UnityEngine;

public class SettingsMenuController
{
    private SettingsMenuView view;
    private SettingsMenuModel model;

    public System.Action OnBack; // Колбэк назад в главное меню

    public SettingsMenuController(SettingsMenuView view)
    {
        this.view = view;
        this.model = new SettingsMenuModel();

        view.volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        view.backButton.onClick.AddListener(OnBackClicked);

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        model.volume = savedVolume;
        view.volumeSlider.value = savedVolume;

        GameBootstrapper.Instance.AudioService.SetVolume(savedVolume);
    }

    private void OnVolumeChanged(float value)
    {
        model.volume = value;
        GameBootstrapper.Instance.AudioService.SetVolume(value);
        PlayerPrefs.SetFloat("Volume", value);
    }

    private void OnBackClicked()
    {
        view.panelRoot.SetActive(false);
        OnBack?.Invoke();
    }

    public void Show()
    {
        view.panelRoot.SetActive(true);
    }
}