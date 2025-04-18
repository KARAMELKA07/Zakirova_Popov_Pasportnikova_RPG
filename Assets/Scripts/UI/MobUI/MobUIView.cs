using UnityEngine;
using UnityEngine.UI;

public class MobUIView : MonoBehaviour
{
    public Slider hpBar;
    public Transform mob; // к кому привязан UI
    public Vector3 offset = new Vector3(0, 1f, 0);

    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    public void UpdatePosition()
    {
        if (mob == null) return;

        transform.position = mob.position + offset;
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0); // чтобы не был задом
    }

    public void UpdateHP(float percent)
    {
        if (hpBar != null)
            hpBar.value = percent;
    }
}