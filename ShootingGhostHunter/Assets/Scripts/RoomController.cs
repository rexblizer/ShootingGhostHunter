using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private bool roomCleared;
    [SerializeField] private int spawnerAmount;
    [SerializeField] private int spawnersDone;

    [SerializeField] private Transform spawner1;
    [SerializeField] private Transform spawner2;
    [SerializeField] private Transform spawner3;
    [SerializeField] private Transform spawner4;

    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            RoomActivation();
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            RoomDeactivation();
        }
    }
    private void RoomActivation()
    {
        if (roomCleared == false)
        {
            Invoke("CloseDoors", 1f);
            spawner1.GetComponent<EnemySpawner>().SpawnerActivate();
            spawner2.GetComponent<EnemySpawner>().SpawnerActivate();
            spawner3.GetComponent<EnemySpawner>().SpawnerActivate();
            spawner4.GetComponent<EnemySpawner>().SpawnerActivate();
        }
    }

    private void RoomDeactivation()
    {
        Invoke("OpenDoors", 1f);
        spawner1.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner2.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner3.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner4.GetComponent<EnemySpawner>().SpawnerDeactivate();
    }

    private void CloseDoors()
    {
        if (door1 != null)
        {
            door1.SetActive(true);
        }
        if (door2 != null)
        {
            door2.SetActive(true);
        }
    }

    private void OpenDoors()
    {
        if (door1 != null)
        {
            door1.SetActive(false);
        }
        if (door2 != null)
        {
            door2.SetActive(false);
        }
    }
    public void OneSpawnerDone()
    {
        spawnersDone = spawnersDone + 1;
        if (spawnersDone == spawnerAmount)
        {
            roomCleared = true;
            RoomDeactivation();
        }
    }
}
