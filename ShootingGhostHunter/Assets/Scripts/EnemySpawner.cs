using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform enemyPF;
    [SerializeField] private int spawnAmount = 10;
    [SerializeField] private int spawnDelay = 3;

    private void Awake()
    {
        if(spawnAmount == 0)
        {
            spawnAmount = Random.Range(1, 6);
        }
        Invoke("Spawn", spawnDelay);
    }
    public void Spawn()
    {
        if (spawnAmount > 0)
        {
            spawnAmount = spawnAmount - 1;
            Instantiate(enemyPF);
            Invoke("Spawn", spawnDelay);
        }
    }
}
