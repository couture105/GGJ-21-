using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    bool _initialized = false;

    public int maxSheeps = 128;
    public int maxWolfs = 8;
    public int maxRams = 8;
    public float shepherdSpawnRadius = 10.0f;

    public GameObject deathEffectPrefab;

    public Transform effectsParent;
    public Transform sheepsParent;
    public Transform wolfsParent;
    public Transform ramsParent;
    public Transform shepherdParent;
    public Camera mainCamera;
    public float cameraSpeed = 5.0f;

    public List<Sheep> sheeps;
    public List<Wolf> wolfs;
    public List<Ram> rams;
    public Shepherd shepherd;

    public List<SheepSpawner> sheepSpawners;
    public List<WolfSpawner> wolfSpawners;
    public List<RamSpawner> ramSpawners;
    public ShepherdSpawner shepherdSpawner;

    public List<Pen> pens;

    public int activeSheeps = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.level = this;
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
        foreach (Ram ram in rams)
        {
            ram.DeltaUpdate(Time.deltaTime);
        }

        if (pens != null)
        {
            foreach (Pen pen in pens)
            {
                pen.DeltaUpdate(Time.deltaTime);
            }
        }
    }

    void Initialize()
    {
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
        activeSheeps = 0;
        maxSheeps = 0;

        if (sheepSpawners != null)
        {
            sheepSpawners.Clear();
            sheepSpawners = null;
        }
        sheepSpawners = FindObjectsOfType<SheepSpawner>().ToList();
        if (sheepSpawners != null)
        {
            foreach (SheepSpawner spawner in sheepSpawners)
            {
                spawner.level = this;
                maxSheeps += spawner.maxSheeps;
                spawner.SpwanSheeps();
            }
        }

        if (wolfs != null)
        {
            foreach (Wolf wolf in wolfs)
            {
                GameObject.Destroy(wolf.gameObject);
            }

            wolfs.Clear();
            wolfs = null;
        }
        wolfs = new List<Wolf>();
        maxWolfs = 0;

        if (wolfSpawners != null)
        {
            wolfSpawners.Clear();
            wolfSpawners = null;
        }
        wolfSpawners = FindObjectsOfType<WolfSpawner>().ToList();
        if (wolfSpawners != null)
        {
            foreach (WolfSpawner spawner in wolfSpawners)
            {
                spawner.level = this;
                maxWolfs += spawner.maxWolfs;
                spawner.SpawnWolfs();
            }
        }

        if (rams != null)
        {
            foreach (Ram ram in rams)
            {
                GameObject.Destroy(ram.gameObject);
            }

            rams.Clear();
            rams = null;
        }
        rams = new List<Ram>();
        maxRams= 0;

        if (ramSpawners != null)
        {
            ramSpawners.Clear();
            ramSpawners = null;
        }
        ramSpawners = FindObjectsOfType<RamSpawner>().ToList();
        if (ramSpawners != null)
        {
            foreach (RamSpawner spawner in ramSpawners)
            {
                spawner.level = this;
                maxRams += spawner.maxRams;
                spawner.SpawnRams();
            }
        }

        if (pens != null)
        {
            pens.Clear();
            pens = null;
        }
        pens = FindObjectsOfType<Pen>().ToList();
        if (pens != null)
        {
            foreach (Pen pen in pens)
            {
                pen.level = this;
            }
        }

        if (shepherd != null)
        {
            GameObject.Destroy(shepherd.gameObject);
            shepherd = null;
        }
        if (shepherdSpawner != null)
        {
            shepherdSpawner = null;
        }
        shepherdSpawner = FindObjectOfType<ShepherdSpawner>();
        if (shepherdSpawner != null)
        {
            shepherdSpawner.level = this;
            shepherdSpawner.SpawnShepherd();
        }   

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
                if (sheep.respawns)
                {
                    sheep.transform.position = sheep.startPos;
                    sheep.active = true;
                    activeSheeps++;
                }
                else
                {
                    int startIndex = Random.Range(0, sheepSpawners.Count);
                    bool spawned = false;
                    for (int j = startIndex; j < sheepSpawners.Count; j++)
                    {
                        if (sheepSpawners[j].respawns && (sheepSpawners[j].sheepType == sheep.type))
                        {
                            sheepSpawners[j].RespawnSheep(sheep);
                            spawned = true;
                            break;
                        }
                    }
                    if (!spawned)
                    {
                        for (int j = 0; j < startIndex; j++)
                        {
                            if (sheepSpawners[j].respawns && (sheepSpawners[j].sheepType == sheep.type))
                            {
                                sheepSpawners[j].RespawnSheep(sheep);
                                spawned = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddShepherd(Shepherd shep)
    {
        shep.level = this;
        shepherd = shep;
    }

    public void AddWolf(Wolf wolf)
    {
        wolf.level = this;
        wolfs.Add(wolf);
    }

    public void AddRam(Ram ram)
    {
        ram.level = this;
        rams.Add(ram);
    }

    public void AddSheep(Sheep sheep)
    {
        sheep.level = this;
        activeSheeps++;
        sheeps.Add(sheep);
    }

    public void DestroySheep(Sheep sheep)
    {
        sheep.active = false;
        activeSheeps--;
    }

    public void SpawnDeathEffect(Vector3 pos, Quaternion rot)
    {
        if (deathEffectPrefab != null && effectsParent != null)
        {
            GameObject effect = GameObject.Instantiate(deathEffectPrefab, effectsParent);
            effect.transform.position = pos;
            effect.transform.rotation = rot;
        }
    }
}
