using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public static MobManager Instance { get; private set; }

    private List<GameObject> activeMobs = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterMob(GameObject mob)
    {
        activeMobs.Add(mob);
    }

    public void UnregisterMob(GameObject mob)
    {
        activeMobs.Remove(mob);
        Destroy(mob, 3f);
    }

    public int GetMobCount()
    {
        return activeMobs.Count;
    }
}