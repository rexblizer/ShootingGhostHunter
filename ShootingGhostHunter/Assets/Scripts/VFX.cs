using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private Vector3 VFXDir;
    [SerializeField] private Transform player;

    public void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
    }
    void Update()
    {
        transform.LookAt(player);
        Destroy(gameObject, destroyTime);
    }
}