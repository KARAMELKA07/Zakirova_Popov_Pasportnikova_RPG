using UnityEngine;

public class GameplayBootstrapper : MonoBehaviour
{
    public PauseMenuView pauseMenuView;
    private PauseMenuController controller;

    void Start()
    {
        controller = new PauseMenuController(pauseMenuView);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controller.ToggleMenu();
        }
    }
}