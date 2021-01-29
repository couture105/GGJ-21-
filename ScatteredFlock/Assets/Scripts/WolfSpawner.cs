using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    public float spawnRadius = 8.0f;
    public GameObject wolfPrefab;
    public int maxWolfs;
    public Level level;

    public void SpawnWolfs()
    {
        for (int i = 0; i < maxWolfs; i++)
        {
            Wolf wolf = GameObject.Instantiate(wolfPrefab, level.wolfsParent).GetComponent<Wolf>();
            wolf.active = true;
            level.AddWolf(wolf);
            wolf.transform.position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
            wolf.startPos = wolf.transform.position;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Wolf.png", true);
    }
}
