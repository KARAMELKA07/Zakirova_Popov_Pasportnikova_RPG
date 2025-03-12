using UnityEngine;
using UnityEngine.UI;

public class MobUI : MonoBehaviour
{
    public Slider hpBar; // Полоска HP
    public MobHealth mobHealth; // Скрипт здоровья моба
    public Transform mob; // Сам моб
    public Vector3 offset = new Vector3(0, 1f, 0); // Смещение над головой

    private Transform playerCamera; // Камера игрока

    void Start()
    {
        playerCamera = Camera.main.transform; // Автоматически находим камеру
    }

    void Update()
    {
        if (mob == null || hpBar == null) return;

        // Обновляем HP
        hpBar.value = (float)mobHealth.GetCurrentHP() / mobHealth.maxHP;

        // Размещаем UI над мобом
        transform.position = mob.position + offset;

        // Поворачиваем UI к игроку
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0); // Разворачиваем, так как LookAt делает инверсию
    }
}