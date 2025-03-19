using UnityEngine;
using UnityEngine.UI;

public class MobUI : MonoBehaviour
{
    public Slider hpBar;
    public MobHealth mobHealth; 
    public Transform mob; 
    public Vector3 offset = new Vector3(0, 1f, 0); 

    private Transform playerCamera; 

    void Start()
    {
        playerCamera = Camera.main.transform; 
    }

    void Update()
    {
        if (mob == null || hpBar == null) return;

        hpBar.value = (float)mobHealth.GetCurrentHP() / mobHealth.maxHP;

        transform.position = mob.position + offset;

        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0); 
    }
}