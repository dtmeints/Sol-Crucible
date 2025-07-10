using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="newLevel", menuName = "Data/Level")]
public class D_Level : ScriptableObject
{
    [SerializeField] string text1;
    [SerializeField] string text2;
    [SerializeField] Color color = Element.Fire.Color();
    [SerializeField] Requirements requirements;
    [SerializeField] Spawn[] spawns;
    [SerializeField] GameObject wallsPrefab;
    [SerializeField] int challengeLevel;
    public string Text1 => text1;
    public string Text2 => text2;
    public Requirements Requirements => requirements;
    public Spawn[] Spawns => spawns;
    public Color Color => color;
    public GameObject WallsPrefab => wallsPrefab;
    public int ChallengeLevel => challengeLevel;

    public int LevelIndex()
    {
        for (int i = 0; i < GameManager.Instance.Levels.Length; i++)
        {
            D_Level level = GameManager.Instance.Levels[i];
            if (level == this)
                return i;
        }
        Debug.Log("Level not found");
        return -1;
    }
}

[Serializable]
public class Spawn
{
    public Element element;
    public int spawnPoint;
    public int rank = 1;
}


