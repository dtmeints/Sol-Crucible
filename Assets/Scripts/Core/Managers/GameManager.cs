using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<GameManager>();
            return instance;
        }
    }

    public bool GameStarted { get; private set; }
    public bool GameEnded { get; private set; }
    [SerializeField] D_Level[] levels;
    public D_Level[] Levels => levels;
    public bool IsInLevelSequence => CurrentLevelIndex >= 0;
    public int CurrentLevelIndex;
    public D_Level CurrentLevel;

    public Stats Stats { get; private set; }

    public static event Action GameOver;
    
    void Reset() => CurrentLevel = Resources.Load<D_Level>("Data Objects/Levels/ThrowOrb.LEVEL.asset");
    private void Awake()
    {
        CurrentLevel = Resources.Load<D_Level>("Data Objects/Levels/ThrowOrb.LEVEL.asset");
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Stats = new Stats();
    }

    private void Update()
    {
        if (!GameEnded)
            Stats.duration += Time.deltaTime;
    }

    public async Task StartLevelInSequence(int levelIndex)
    {
        if (levels[levelIndex] != null)
        {
            await StartLevel(levels[levelIndex]);
        }
    }

    public async Task StartLevel(D_Level level)
    {
        GameStarted = false;
        CurrentLevel = level;
        CurrentLevelIndex = level.LevelIndex();
        Stats = new Stats();
        await SceneManager.LoadSceneAsync("Gameplay");

        if (CurrentLevel.WallsPrefab != null)
            Instantiate(CurrentLevel.WallsPrefab);

        if (CurrentLevel.ChallengeLevel > 0)
        {
            WallController walls = FindFirstObjectByType<WallController>();
            if (walls != null) walls.SetAllChildLimits(CurrentLevel.ChallengeLevel);
        }    
        await Awaitable.WaitForSecondsAsync(1f);
        GameEnded = false;
        
        GameStarted = true;
    }

    public void EndGame()
    {
        Debug.Log("Ending Game");
        if (GameEnded && !GameStarted)
            return;

        GameEnded = true;
        GameOver?.Invoke();
        AudioManager.Instance.PlayVictory();

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
            wall.SetActive(false);   

        OrbManager.Instance.FlingAll();

        UILevelEndMenu levelEndMenu = FindAnyObjectByType<UILevelEndMenu>(FindObjectsInactive.Include);
        if (levelEndMenu != null)
            levelEndMenu.EndLevel();
    }
}
