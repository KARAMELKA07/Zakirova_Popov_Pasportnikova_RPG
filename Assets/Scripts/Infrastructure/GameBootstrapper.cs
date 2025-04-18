using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    public ISaveService SaveService { get; private set; }
    public IAudioService AudioService { get; private set; }
    public IGameStateService GameStateService { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Инициализация сервисов
        SaveService = new FileSaveService();
        AudioService = new AudioService();
        GameStateService = new GameStateService();
    }
}