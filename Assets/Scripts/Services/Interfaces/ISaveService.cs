public interface ISaveService
{
    void SaveGame(GameData data);
    GameData LoadGame();
}