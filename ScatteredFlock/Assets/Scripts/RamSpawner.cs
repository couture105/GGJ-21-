using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RamSpawner : MonoBehaviour
{
    public float spawnRadius = 8.0f;
    public GameObject ramPrefab;
    public int maxRams;
    public Level level;

    public void SpawnRams()
    {
        for (int i = 0; i < maxRams; i++)
        {
            Ram ram = GameObject.Instantiate(ramPrefab, level.ramsParent).GetComponent<Ram>();
            ram.active = true;
            level.AddRam(ram);
            ram.transform.position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
            ram.startPos = ram.transform.position;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Ram.png", true);
    }
}

