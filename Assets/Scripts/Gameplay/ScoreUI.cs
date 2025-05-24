using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }

    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateScore(int kills)
    {
        scoreText.text = $"Счёт: {kills}";
    }
    void Start()
    {
        UpdateScore(0);
    }

}