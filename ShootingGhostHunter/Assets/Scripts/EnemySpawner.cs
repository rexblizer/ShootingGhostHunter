using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemyPF;

    public bool canSpawnSword;
    public bool canSpawnArcher;
    public bool canSpawnTank;
    public bool canSpawnMage;
    public int spawnableClasses;

    [SerializeField] private int spawnAmount = 10;
    [SerializeField] private int spawnDelay = 3;
    [SerializeField] private int spawnStartDelay = 1;

    [SerializeField] private int maxSpawnAmount;
    [SerializeField] private int enemysKilled;
    public bool spawnerDone = false;

    private void Awake()
    {
        DeterminSpawnableClasses();
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
        Invoke("Spawn", 1f);
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
    private void DeterminSpawnableClasses()
    {
        if (canSpawnSword) spawnableClasses = spawnableClasses + 1;
        if (canSpawnArcher) spawnableClasses = spawnableClasses + 1;
        if (canSpawnTank) spawnableClasses = spawnableClasses + 1;
        if (canSpawnMage) spawnableClasses = spawnableClasses + 1;
    }
}
