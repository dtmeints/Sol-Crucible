using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.Splines.SplineInstantiate;

public class Orb : MonoBehaviour, IClickable
{
    [Header("Components")]
    [SerializeField] TMPro.TextMeshPro rankText;

    private float lastRankUpdateTime;
    [SerializeField] Rigidbody2D rb;
    public Rigidbody2D RB => rb;
    MaterialPropertyBlock MPB;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer symbolSR;
    [SerializeField] Collider2D col;
    [SerializeField] LineRenderer chain;
    public SpriteRenderer SR => sr;

    [Header("Settings")]
    [SerializeField] Element element;
    public Element Element { get { return element; } set { element = value; } }
    [SerializeField] float maxImpactSpeed = 5;
    public float resizeSpeed = 4;
    [ColorUsage(true, true), SerializeField] Color flashColorA, flashColorB;
    [SerializeField] D_RankSizingTable sizingTable;
    [SerializeField] D_ElementLibrary elementLibrary;

    float spawnTime;
    public float LastReactionTime { get; private set; }

    [Header("Gameplay")]
    public Orb twinOriginal;
    public float timeOfTwinning;
    public int Rank;
    [SerializeField] int visibleRank;
    Transform destination;

    private float distanceTimer;

    private void Awake()
    {
        MPB = new MaterialPropertyBlock();
        col.isTrigger = false;
        spawnTime = Time.time;
        LastReactionTime = Time.time;
        if (gameObject.activeInHierarchy)
            StartCoroutine(Co_Flash(Mathf.Clamp(1, 0, 1)));
    }

    private void Update()
    {
        if (Time.time - lastRankUpdateTime > .05f && visibleRank != Rank)
            UpdateVisibleRank();

        //changes length and direction of chain
        ManageChainGraphics();

        //gradually grow or shrink to rank
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * sizingTable.GetSize(Rank), resizeSpeed * Time.deltaTime);

        //if being consumed, move toward destination
        if (destination)
            transform.position = Vector2.MoveTowards(transform.position, destination.position, 5 * Time.deltaTime);

        CheckIfShouldBeEliminated();

        distanceTimer += Time.deltaTime;
        if (distanceTimer >= 1f)
        {
            CheckDistance();
        }
    }

    private void ManageChainGraphics()
    {
        //show chain if the original orb I was twinned from is alive
        //if I'm being consumed or the original has been, shrink the chain
        if (chain != null && twinOriginal != null && twinOriginal.Rank > 0 && Rank > 0)
        {
            chain.gameObject.SetActive(true);
            Vector3 midpoint = .5f * (transform.position + twinOriginal.transform.position);
            chain.SetPositions(new Vector3[] { transform.position, midpoint, twinOriginal.transform.position });
        }
        else
        {
            twinOriginal = null;
            if (chain.gameObject.activeInHierarchy)
            {
                Vector3[] positions = new Vector3[chain.positionCount];
                chain.GetPositions(positions);
                positions[^1] = Vector3.MoveTowards(positions[^1], positions[0], 20 * Time.deltaTime);
                positions[1] = .5f * (positions[0] + positions[^1]);
                chain.SetPositions(positions);
                //if the chain is fully shrunk, deactivate
                if (((positions[0] - positions[^1]).sqrMagnitude) < .2f)
                    chain.gameObject.SetActive(false);
            }
        }
    }

    public void Initialize(Element element, int rank)
    {
        Element = element;
        SetRank(rank);
        UpdateVisibleRank();
        destination = null;
        transform.parent = null;
        RB.linearVelocity = Vector2.zero;
        if (GameManager.Instance != null)
        {
            RB.linearDamping = OrbManager.Instance.OrbLinearDamp;
        }
        else
        {

        }
        RB.simulated = true;
        col.isTrigger = false;
        twinOriginal = null;
        spawnTime = Time.time;
        LastReactionTime = Time.time;
        rankText.text = TextUtil.ToRoman(Rank);
        SR.color = elementLibrary.GetColor(element);
        symbolSR.sprite = elementLibrary.GetSymbol(element);
        gameObject.name = element + " Orb";

        for (int i = 0; i < chain.colorGradient.colorKeys.Length; i++)
        {
            chain.colorGradient.colorKeys[i].color = elementLibrary.GetColor(element);
        }
        StartCoroutine(Co_Flash(Mathf.Clamp(1, 0, 1)));
    }
    private void CheckIfShouldBeEliminated()
    {
        if (visibleRank != 0 || Rank != 0)
            return;

        col.isTrigger = true;

        //STILL SHRINKING
        if (transform.localScale.x > .01f)
            return;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(enableWait());
    }

    IEnumerator enableWait()
    {
        yield return new WaitForSeconds(0.01f);
        OrbManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (OrbManager.Instance)
            OrbManager.Instance.Unregister(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactMagnitude = collision.relativeVelocity.magnitude;
        AudioManager.Instance.PlayCollisionSound(impactMagnitude, collision.contacts[0].point);

        if (collision.gameObject.TryGetComponent<Orb>(out var orb)
            && Time.time - spawnTime > .5f
            && impactMagnitude > OrbManager.Instance.MinimumImpactForce)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(Co_Flash(Mathf.Clamp(collision.relativeVelocity.magnitude / maxImpactSpeed, 0, 1)));

            if (Time.time - LastReactionTime < .1f || Time.time - orb.LastReactionTime < .1f)
                return;


            if (Reactions.React(this, orb, collision.relativeVelocity))
                LastReactionTime = Time.time;
        }
    }

    public void Push(Vector2 velocity)
    {
        RB.linearVelocity = velocity;
        StartCoroutine(Co_Flash((velocity.magnitude + 15) / 30f));
    }

    public void SetRank(int rank)
    {
        if (rank > Rank)
            if (GameManager.Instance != null) { GameManager.Instance.Stats.AddCreatedElement(rank - Rank, Element); }

        Rank = rank;
    }

    public void AddRank(int rank)
    {
        if (Rank + rank <= 0)
            SetRank(1);
        else SetRank(Rank + rank);
    }

    public void BeConsumed(Transform destination)
    {
        SetRank(0);
        this.destination = destination;
        transform.parent = destination;
        RB.simulated = false;
    }

    private void UpdateVisibleRank()
    {
        visibleRank += visibleRank - Rank > 0 ? -1 : 1;

        rankText.text = TextUtil.ToRoman(visibleRank);
        RB.mass = sizingTable.GetSize(visibleRank) * 9 + .1f;
        lastRankUpdateTime = Time.time;
    }

    public IEnumerator Co_Flash(float intensity)
    {
        float startTime = Time.time;
        float duration = .2f;

        while (Time.time - startTime < duration)
        {
            MPB.SetColor("_AddColor", Color.Lerp(flashColorA, flashColorB, (Time.time - startTime) / duration) * intensity);
            SR.SetPropertyBlock(MPB);
            yield return null;
        }

        duration = .5f;
        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            MPB.SetColor("_AddColor", Color.Lerp(flashColorB, flashColorA, (Time.time - startTime) / duration) * intensity);
            SR.SetPropertyBlock(MPB);
            yield return null;
        }

        MPB.SetColor("_AddColor", flashColorA);
        SR.SetPropertyBlock(MPB);
    }

    public void ActivateVFX()
    {

    }

    void IClickable.ActivateClickEffect()
    {
    }

    void IClickable.ActivateMouseOverEffect()
    {
        StartCoroutine(Co_Flash(.5f));
    }


    void CheckDistance()
    {
        if (Vector3.Distance(transform.position, OrbManager.Instance.transform.position) >= 50f)
        {
            Debug.Log("Orb out of bounds");
            SetRank(0);
        }
    }
}
