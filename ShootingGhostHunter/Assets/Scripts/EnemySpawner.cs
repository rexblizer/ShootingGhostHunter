using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemyPF;
    [SerializeField] private int spawnAmount = 10;
    [SerializeField] private int spawnDelay = 3;

    [SerializeField] private int maxSpawnAmount;
    [SerializeField] private int enemysKilled;
    public bool spawnerDone = false;

    private void Awake()
    {
        if(spawnAmount == 0)
        {
            spawnAmount = Random.Range(1, 6);
        }
        maxSpawnAmount = spawnAmount;
    }
    private void Spawn()
    {
        if (spawnAmount > 0)
        {
            spawnAmount = spawnAmount - 1;
            Instantiate(enemyPF, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            Invoke("Spawn", spawnDelay);
        }
    }
    public void SpawnerActivate()
    {
        Invoke("Spawn", spawnDelay);
    }
    public void SpawnerDeactivate()
    {
        CancelInvoke("Spawn");
    }
    public void EnemyKilled()
    {
        enemysKilled = enemysKilled + 1;
        if (maxSpawnAmount == enemysKilled)
        {
            spawnerDone = true;
        }
        if (spawnerDone == true)
        {
            GetComponentInParent<RoomController>().OneSpawnerDone();
            spawnerDone = false;
        }
    }
}
