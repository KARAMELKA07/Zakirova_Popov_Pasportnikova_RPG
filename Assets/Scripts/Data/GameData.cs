using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public int playerHP;

    // доп. баллы
    public List<MobSaveData> mobs = new();
}

[System.Serializable]
public class MobSaveData
{
    public string mobType;
    public Vector3 position;
    public int hp;
}