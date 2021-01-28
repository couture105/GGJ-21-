using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    bool _initialized = false;

    public GameObject sheepPrefab;
    public GameObject wolfPrefab;
    public GameObject shepherdPrefab;

    public GameObject pen;

    public int maxSheeps = 128;
    public int maxWolfs = 8;
    public float penRadius = 2.0f;
    public float shepherdSpawnRadius = 10.0f;
    public float sheepSpawnRadius = 24.0f;
    public float wolfSpawnRadius = 32.0f;

    public Transform sheepsParent;
    public Transform wolfsParent;
    public Transform shepherdParent;
    public Camera mainCamera;
    public float cameraSpeed = 5.0f;

    public List<Sheep> sheeps;
    public List<Wolf> wolfs;
    public Shepherd shepherd;

    public List<SheepSpawner> sheepSpawners;
    public List<WolfSpawner> wolfSpawners;
    int wolfSpawnerIndex = 0;
    int sheepSpawnerIndex = 0;

    public int activeSheeps = 0;
    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_initialized)
        {
            Initialize();
        }

        Vector2 cameraPosition = Vector2.MoveTowards(mainCamera.transform.position, shepherd.transform.position, cameraSpeed * Time.deltaTime);
        mainCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, mainCamera.transform.position.z);

        SpwanSheeps();
        shepherd.DeltaUpdate(Time.deltaTime);
        foreach (Sheep sheep in sheeps)
        {
            sheep.DeltaUpdate(Time.deltaTime);
        }
        foreach (Wolf wolf in wolfs)
        {
            wolf.DeltaUpdate(Time.deltaTime);
        }
        PenSheeps();
    }

    void Initialize()
    {
        if (sheepSpawners != null)
        {
            sheepSpawners.Clear();
            sheepSpawners = null;
        }
        sheepSpawners = FindObjectsOfType<SheepSpawner>().ToList();

        if (wolfSpawners != null)
        {
            wolfSpawners.Clear();
            wolfSpawners = null;
        }
        wolfSpawners = FindObjectsOfType<WolfSpawner>().ToList();

        if (shepherd != null)
        {
            GameObject.Destroy(shepherd.gameObject);
            shepherd = null;
        }
        shepherd = GameObject.Instantiate(shepherdPrefab, shepherdParent).GetComponent<Shepherd>();
        shepherd.transform.position = new Vector3(Random.Range(-shepherdSpawnRadius, shepherdSpawnRadius), Random.Range(-shepherdSpawnRadius, shepherdSpawnRadius), 0);
        shepherd.active = true;
        shepherd.level = this;

        if (sheeps != null)
        {
            foreach (Sheep sheep in sheeps)
            {
                GameObject.Destroy(sheep.gameObject);
            }

            sheeps.Clear();
            sheeps = null;
        }

        sheeps = new List<Sheep>();
        for (int i = 0; i < maxSheeps; i++)
        {
            Sheep sheep = GameObject.Instantiate(sheepPrefab, sheepsParent).GetComponent<Sheep>();
            sheep.active = false;
            sheep.level = this;
            sheeps.Add(sheep);
        }
        activeSheeps = 0;
        score = 0;

        wolfs = new List<Wolf>();
        for (int i = 0; i < maxWolfs; i++)
        {
            Wolf wolf = GameObject.Instantiate(wolfPrefab, wolfsParent).GetComponent<Wolf>();
            
            wolf.active = true;
            wolf.level = this;
            wolfs.Add(wolf);

            if (wolfSpawners != null && wolfSpawners.Count > 0)
            {
                WolfSpawner spawner = wolfSpawners[wolfSpawnerIndex];
                wolf.transform.position = spawner.transform.position + 
                    new Vector3(Random.Range(-spawner.spawnRadius, spawner.spawnRadius), Random.Range(-spawner.spawnRadius, spawner.spawnRadius), 0);
                wolfSpawnerIndex++;
                if (wolfSpawnerIndex >= wolfSpawners.Count)
                {
                    wolfSpawnerIndex = 0;
                }
            }
            else
            {
                wolf.transform.position = new Vector3(Random.Range(-wolfSpawnRadius, wolfSpawnRadius), Random.Range(-wolfSpawnRadius, wolfSpawnRadius), 0);
            }

            wolf.startPos = wolf.transform.position;
        }
        score = 0;

        _initialized = true;
    }

    void SpwanSheeps()
    {
        for (int i = 0; i < maxSheeps; i++)
        {
            if (activeSheeps >= maxSheeps)
            {
                break;
            }

            Sheep sheep = sheeps[i];
            if (!sheep.active)
            {
                if (sheepSpawners != null && sheepSpawners.Count > 0)
                {
                    SheepSpawner spawner = sheepSpawners[sheepSpawnerIndex];
                    sheep.transform.position = spawner.transform.position +
                        new Vector3(Random.Range(-spawner.spawnRadius, spawner.spawnRadius), Random.Range(-spawner.spawnRadius, spawner.spawnRadius), 0);
                    sheepSpawnerIndex++;
                    if (sheepSpawnerIndex >= sheepSpawners.Count)
                    {
                        sheepSpawnerIndex = 0;
                    }
                }
                else
                {
                    sheep.transform.position = new Vector3(Random.Range(-sheepSpawnRadius, sheepSpawnRadius),
                        Random.Range(-sheepSpawnRadius, sheepSpawnRadius), 0);
                }
            
                sheep.active = true;
                activeSheeps++;
            }
        }
    }

    void PenSheeps()
    {
        for (int i = 0; i < maxSheeps; i++)
        {
            Sheep sheep = sheeps[i];
            if (sheep.active)
            {
                if ((sheep.transform.position - pen.transform.position).magnitude < penRadius)
                {
                    sheep.active = false;
                    activeSheeps--;
                    score++;
                }
            }
        }
    }
}
