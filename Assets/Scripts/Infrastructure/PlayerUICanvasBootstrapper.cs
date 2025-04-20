using UnityEngine;

public class PlayerUICanvasBootstrapper : MonoBehaviour
{
    public PlayerUIView view;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;

    private PlayerUIController controller;

    void Start()
    {
        controller = new PlayerUIController(view, playerHealth, playerMovement);
    }

    void Update()
    {
        controller.Update();
    }
}