using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class OrbManager : MonoBehaviour
{
    public Transform OrbParent;

    private static OrbManager instance;
    public static OrbManager Instance
    {
        get
        {
            if (instance == null && GameManager.Instance != null)
                instance = FindAnyObjectByType<OrbManager>();
            return instance;
        }
        private set { return;}
    }

    public bool isActive;
    public D_Level AllElementsLevel;

    private HashSet<Orb> orbs = new();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] Orb orbPrefab;
    public D_ElementLibrary elementLibrary;

    public int Count => orbs.Count;
    public int MaxOrbCount = 40;
    public float OrbLinearDamp = .4f;
    public float OrbResizeSpeed = 4f;
    public float MinimumImpactForce = 2f;
    private List<Orb> pooledOrbs = new List<Orb>();


    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;
        }
    }

    public void SetInstance()
    {
        instance = this;
        Instance = instance;
    }

    public void UnsetInstance()
    {
        instance = null;
        Instance = instance;
    }

    private void Start()
    {
        for (int i = 0; i < MaxOrbCount; i++)
        {
            Orb newOrb = (Orb)Instantiate(orbPrefab, transform);
            newOrb.gameObject.transform.parent = OrbParent;
            pooledOrbs.Add(newOrb);
            newOrb.gameObject.SetActive(false);
        }
        if (GameManager.Instance != null)
        {
            SpawnOrbs(GameManager.Instance.CurrentLevel.Spawns);
            _ = MissingElementCheck();
        }
        else
        {
            SpawnOrbs(AllElementsLevel.Spawns);
            _ = MissingElementCheck();
        }
    }


    private async Task MissingElementCheck()
    {
        await Awaitable.WaitForSecondsAsync(1);

        if (GameManager.Instance != null)
        {

            if (GameManager.Instance.GameEnded)
                return;

            HashSet<Element> elements = new HashSet<Element>();
            foreach (var orb in pooledOrbs)
            {
                if (orb.gameObject.activeInHierarchy)
                    elements.Add(orb.Element);
            }
            foreach (var spawn in GameManager.Instance.CurrentLevel.Spawns)
            {
                if (!elements.Contains(spawn.element))
                {
                    Vector2 spawnPoint = await GetRandomSpawnPoint();
                    SpawnOrb(spawn.element, 1, spawnPoint);
                }
            }
            _ = MissingElementCheck();

        }
        else if(isActive)
        {
            HashSet<Element> elements = new HashSet<Element>();
            foreach (var orb in pooledOrbs)
            {
                if (orb.gameObject.activeInHierarchy)
                    elements.Add(orb.Element);
            }
            foreach (var spawn in AllElementsLevel.Spawns)
            {
                if (!elements.Contains(spawn.element))
                {
                    Vector2 spawnPoint = await GetRandomSpawnPoint();
                    SpawnOrb(spawn.element, 1, spawnPoint);
                }
            }
            _ = MissingElementCheck();
        }
    }

    public void Register(Orb orb)
    {
        orbs.Add(orb);
    }

    public void Unregister(Orb orb)
    {
        if (orbs.Contains(orb))
            orbs.Remove(orb);
    }

    public void SpawnOrbs(Spawn[] spawns)
    {
        Debug.Log("Spawning");
        foreach (Spawn spawn in spawns)
        {
            SpawnOrb(spawn.element, spawn.rank, spawnPoints[spawn.spawnPoint].position);
        }
    }

    public Orb SpawnOrb(Element element, int rank, Vector3 position)
    {
        Orb newOrb = GetPooledOrb();
        newOrb.gameObject.SetActive(true);
        newOrb.Initialize(element, rank);
        newOrb.transform.position = position;
        newOrb.gameObject.transform.parent = OrbParent;
        return newOrb;
    }

    public bool IsOnlyRemainingOrbOfElement(Orb orb)
    {
        foreach (Orb b in orbs)
        {
            if (b.Element == orb.Element && b.gameObject.activeSelf && b != orb)
                return false;
        }
        return true;
    }

    public async Task<Vector2> GetRandomSpawnPoint()
    {
        Vector2 point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
        if (!Physics2D.OverlapCircle(point, .5f))
            return point;
        await Awaitable.WaitForSecondsAsync(.5f);
        return await GetRandomSpawnPoint();
    }

    public void FlingAll()
    {
        foreach (Orb orb in orbs)
        {
            orb.RB.linearVelocity = orb.transform.position.normalized * 10f;
        }
    }

    public void Refresh()
    {
        foreach (Orb orb in orbs)
        {
            orb.RB.linearDamping = OrbLinearDamp;
            orb.resizeSpeed = OrbResizeSpeed;
        }
    }

    public Orb GetPooledOrb()
    {
        foreach (Orb orb in pooledOrbs)
        {
            if (!orb.gameObject.activeInHierarchy)
            {
                return orb;
            }
        }
        return null;
    }
}
