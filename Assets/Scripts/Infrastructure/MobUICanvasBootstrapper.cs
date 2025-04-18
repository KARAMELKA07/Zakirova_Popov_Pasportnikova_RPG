using UnityEngine;

public class MobUICanvasBootstrapper : MonoBehaviour
{
    public MobUIView view;
    public MobHealth mobHealth;

    private MobUIController controller;

    void Start()
    {
        controller = new MobUIController(view, mobHealth);
    }

    void Update()
    {
        controller.Update();
    }
}