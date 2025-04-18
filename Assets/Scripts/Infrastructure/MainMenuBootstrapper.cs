using UnityEngine;

public class MainMenuBootstrapper : MonoBehaviour
{
    public MainMenuView view;

    void Start()
    {
        new MainMenuController(view);
    }
}