using UnityEngine;
using Random = UnityEngine.Random;

public class Planet_Spawner : MonoBehaviour
{
    public GameObject Planet;
    public float TimePerSpawn;

    [Header("Visuals")]
    public Vector2 SizeRange;
    public Sprite[] PlanetTextures;

    private float planetTimer;

    void FixedUpdate()
    {
        planetTimer += Time.deltaTime;

        if (planetTimer >= TimePerSpawn)
        {
            SpawnPlanet();
            planetTimer = 0;
        }
    }


    void SpawnPlanet()
    {
        GameObject newPlanet = Instantiate(Planet);
        int m = Random.Range(0, 2) * 2 - 1;
        newPlanet.transform.position = new Vector3(-55f * m, Random.Range(-15f, 15f), 0f);

        newPlanet.GetComponent<Planet_Movement>().moveDir = new Vector3(1f, 0, 0) * m;

        Sprite texture = PlanetTextures[Random.Range(0, PlanetTextures.Length)];
        float sizeMult = Random.Range(SizeRange.x, SizeRange.y);

        newPlanet.GetComponent<Planet_Script>().RandomVisuals(texture, sizeMult);
    }
}
