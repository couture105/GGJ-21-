using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sheep;

public class SheepSpawner : MonoBehaviour
{
    public float spawnRadius = 8.0f;
    public GameObject sheepPrefab;
    public int maxSheeps;
    public bool respawns = true;
    public SheepType sheepType;
    public Level level;

    public void SpwanSheeps()
    {
        for (int i = 0; i < maxSheeps; i++)
        {
            Sheep sheep = GameObject.Instantiate(sheepPrefab, level.sheepsParent).GetComponent<Sheep>();
            sheep.active = true;
            level.AddSheep(sheep);
            sheep.transform.position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
            sheep.startPos = sheep.transform.position;
            sheep.respawns = respawns;
            sheepType = sheep.type;
        }
    }

    public void RespawnSheep(Sheep sheep)
    {
        sheep.active = true;
        sheep.transform.position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
        sheep.startPos = sheep.transform.position;
        sheep.respawns = respawns;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Sheep.png", true);
    }
}
