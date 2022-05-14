using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private bool roomActive;
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
        roomActive = true;
        CloseDoors();
        spawner1.GetComponent<EnemySpawner>().SpawnerActivate();
        spawner2.GetComponent<EnemySpawner>().SpawnerActivate();
        spawner3.GetComponent<EnemySpawner>().SpawnerActivate();
        spawner4.GetComponent<EnemySpawner>().SpawnerActivate();
    }

    private void RoomDeactivation()
    {
        roomActive = false;
        OpenDoors();
        spawner1.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner2.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner3.GetComponent<EnemySpawner>().SpawnerDeactivate();
        spawner4.GetComponent<EnemySpawner>().SpawnerDeactivate();
    }

    private void CloseDoors()
    {
        door1.SetActive(true);
        door2.SetActive(true);
    }

    private void OpenDoors()
    {
        door1.SetActive(false);
        door2.SetActive(false);
    }
    public void OneSpawnerDone()
    {
        spawnersDone = spawnersDone + 1;
        if (spawnersDone == spawnerAmount)
        {
            RoomDeactivation();
        }
    }
}
