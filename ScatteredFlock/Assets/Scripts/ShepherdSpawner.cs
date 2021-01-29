using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShepherdSpawner : MonoBehaviour
{
    public float spawnRadius = 1.0f;
    public GameObject shepherdPrefab;
    public Level level;

    public void SpawnShepherd()
    {
        Shepherd shepherd = GameObject.Instantiate(shepherdPrefab, level.shepherdParent).GetComponent<Shepherd>();
        shepherd.active = true;
        level.AddShepherd(shepherd);
        shepherd.transform.position = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
        shepherd.startPos = shepherd.transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Shepherd.png", true);
    }
}
