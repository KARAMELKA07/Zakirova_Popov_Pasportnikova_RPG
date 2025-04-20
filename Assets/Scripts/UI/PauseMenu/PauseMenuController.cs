using UnityEngine;

public class PauseMenuController
{
    private PauseMenuView view;

    public PauseMenuController(PauseMenuView view)
    {
        this.view = view;

        view.panelRoot.SetActive(false); // Скрыть меню

        view.mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        view.saveButton.onClick.AddListener(OnSaveClicked);
        view.loadButton.onClick.AddListener(OnLoadClicked);
    }

    public void ToggleMenu()
    {
        bool isActive = view.panelRoot.activeSelf;
        view.panelRoot.SetActive(!isActive);

        // Курсор
        Cursor.visible = !isActive;
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnMainMenuClicked()
    {
        GameBootstrapper.Instance.GameStateService.ReturnToMainMenu();
    }

    private void OnSaveClicked()
    {
        var data = GameDataCollector.Collect();
        GameBootstrapper.Instance.SaveService.SaveGame(data);
        Debug.Log("Сохранение выполнено!");
    }

    private void OnLoadClicked()
    {
        var data = GameBootstrapper.Instance.SaveService.LoadGame();
        if (data != null)
        {
            GameDataRestorer.Restore(data);
            Debug.Log("Загрузка выполнена!");
        }
    }
}